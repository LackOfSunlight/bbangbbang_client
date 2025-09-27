using System;
using System.Collections.Generic;
using DG.Tweening;
using Ironcow;
using TMPro;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.AI;


public class Character : FSMController<CharacterState, CharacterFSM, CharacterDataSO>
{
    // ��� ĳ���͸� �����ϴ�  ����Ʈ
    public static List<Character> allCharacters = new List<Character>();

    [SerializeField] public eCharacterType characterType;
    [SerializeField] private SpriteAnimation anim;
    [SerializeField] private Rigidbody2D rig;
    [SerializeField] private GameObject selectCircle;
    [SerializeField] private SpriteRenderer minimapIcon;
    [SerializeField] private GameObject range;
    [SerializeField] private GameObject targetMark;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject death;
    [SerializeField] private CircleCollider2D collider;
    [SerializeField] public GameObject stop;
    [SerializeField] public GameObject prizon;
    [SerializeField] private TMP_Text damageText;

    [SerializeField] private GameObject bombIcon;
    [SerializeField] private TMP_Text bombTimerText; // ��ź Ÿ�̸� �ؽ�Ʈ

    [SerializeField] private float speed = 3;

    [HideInInspector] public UserInfo userInfo;

    public bool isPlayable { get => characterType == eCharacterType.playable; }
    public Vector2 dir;
    public float Speed { get => speed; }
    public bool isInside;

    private float bombRemainTime = -1; // ��ź Ÿ�̸� (�� ����), -1�� ��Ȱ�� ����

    private void Awake()
    {
        allCharacters.Add(this); // ĳ���� ���

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (characterType == eCharacterType.npc) minimapIcon.gameObject.SetActive(false);

        var renderer = damageText.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "UI"; // ���ϴ� Sorting Layer �̸�
        renderer.sortingOrder = 10;       // ���ڰ� Ŭ���� ��
    }

    // Destroy �� ����Ʈ���� ����
    private void OnDestroy()
    {
        allCharacters.Remove(this);
    }

    public override async void Init(BaseDataSO data)
    {
        this.data = (CharacterDataSO)data;
        fsm = new CharacterFSM(CreateState<CharacterIdleState>().SetElement(anim, rig, this));
        minimapIcon.sprite = await ResourceManager.instance.LoadAsset<Sprite>(data.rcode, eAddressableType.Thumbnail);
    }

    public void SetCharacterType(eCharacterType characterType)
    {
        this.characterType = characterType;
        rig.mass = characterType == eCharacterType.playable ? 10 : 10000;
        agent.enabled = characterType != eCharacterType.playable;
        var tags = CurrentPlayer.ReadOnlyTags();
        if (tags.Length == 0)
        {
            tags = new string[1] { "player1" };
        }
        if (isPlayable)
        {
            if (tags[0].Equals("player1") &&
                (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.WindowsEditor))
            {
                UIGame.instance.stick.gameObject.SetActive(false);
            }
            else
            {
                UIGame.instance.stick.OnHandleChanged += MoveCharacter;
            }
        }
    }

    public void SetMovePosition(Vector3 pos)
    {
        if (characterType == eCharacterType.playable) return;
        //rig.MovePosition(pos);
        agent.SetDestination(pos);
        var isLeft = agent.velocity.x < 0;
        isLeft = data.isLeft ? !isLeft : isLeft;
        if (agent.velocity.x != 0)
            anim.SetFlip(isLeft);
        if (agent.velocity == Vector3.zero)
            OnChangeState<CharacterIdleState>();
        else
            OnChangeState<CharacterWalkState>();
    }

    public void SetPosition(Vector3 pos)
    {
        agent.enabled = false;
        transform.position = pos;
        agent.enabled = true;
    }

    public void OnChangeState<T>() where T : CharacterState
    {
        if (states.ContainsKey(typeof(T).Name))
        {
            ChangeState<T>()?.SetElement(anim, rig, this);
        }
        else
        {
            CreateState<T>()?.SetElement(anim, rig, this);
        }
    }

    public bool IsState<T>()
    {
        return fsm.IsState<T>();
    }

    public void SetTargetMark()
    {
        targetMark.SetActive(true);
    }

    public void OnVisibleMinimapIcon(bool visible)
    {
        if (characterType == eCharacterType.non_playable)
            minimapIcon.gameObject.SetActive(visible && !isInside);
        else
            minimapIcon.gameObject.SetActive(false);
    }

    public void OnSelect()
    {
        selectCircle.SetActive(!selectCircle.activeInHierarchy);
    }

    public void OnVisibleRange()
    {
        range.SetActive(!range.activeInHierarchy);
    }

    float syncFrame = 0;
    public void MoveCharacter(Vector2 dir)
    {
        if (fsm.IsState<CharacterStopState>() || fsm.IsState<CharacterPrisonState>() || fsm.IsState<CharacterDeathState>()) return;
        this.dir = dir;
        var isLeft = dir.x < 0;
        isLeft = data.isLeft ? !isLeft : isLeft;
        if (dir.x != 0)
            anim.SetFlip(isLeft);
        if (dir == Vector2.zero) ChangeState<CharacterIdleState>().SetElement(anim, rig, this);
        else ChangeState<CharacterWalkState>().SetElement(anim, rig, this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            if (characterType == eCharacterType.playable)
            {
                GameManager.instance.SetMapInside(true);
            }
            isInside = true;
            if (userInfo != null)
                OnVisibleMinimapIcon(Util.GetDistance(UserInfo.myInfo.index, userInfo.index, DataManager.instance.users.Count)
                    + userInfo.slotFar <= UserInfo.myInfo.slotRange && userInfo.id != UserInfo.myInfo.id); // ������ �Ÿ��� �ִ� ���� �����ܸ� ǥ��

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Map"))
        {
            if (characterType == eCharacterType.playable)
            {
                GameManager.instance.SetMapInside(false);
            }
            isInside = false;
            if (userInfo != null)
                OnVisibleMinimapIcon(Util.GetDistance(UserInfo.myInfo.index, userInfo.index, DataManager.instance.users.Count)
                    + userInfo.slotFar <= UserInfo.myInfo.slotRange && userInfo.id != UserInfo.myInfo.id); // ������ �Ÿ��� �ִ� ���� �����ܸ� ǥ��
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent<Character>(out var character))
    //    {
    //        if (!SocketManager.instance.isConnected && character == GameManager.instance.userCharacter &&
    //            userInfo.handCards.Find(obj => obj.rcode == "CAD00001"))
    //        {
    //            GameManager.instance.SendSocketUseCard(character.userInfo, userInfo, "CAD00001");
    //        }
    //    }
    //}

    private void Update()
    {
        if (fsm != null)
            fsm.UpdateState();

        // ��ź Ÿ�̸�
        if (bombRemainTime > 0)
        {
            bombRemainTime -= Time.deltaTime;
            int remainInt = Mathf.CeilToInt(bombRemainTime); 
            bombTimerText.text = remainInt.ToString();
        }
        else if (bombRemainTime != -1)
        {   // ���� ó��
            bombRemainTime = -1;
            bombTimerText.text = "00";
            bombTimerText.gameObject.SetActive(false);
        }
    }

    public async void SetDeath()
    {
        death.SetActive(true);
        collider.enabled = false;
        targetMark.SetActive(true);
        targetMark.GetComponent<SpriteRenderer>().sprite = await ResourceManager.instance.LoadAsset<Sprite>("Role_" + userInfo.roleType.ToString(), eAddressableType.Thumbnail);
        minimapIcon.gameObject.SetActive(false);
        ChangeState<CharacterDeathState>();
    }

    protected override T ChangeState<T>()
    {
        if (!IsState<CharacterDeathState>())
            return base.ChangeState<T>();
        else
            return null;
    }

    public void Damage(int damage)
    {
        anim.ChangeSpriteColor(Color.red);
        DOTween.To(() => Color.red, c => anim.ChangeSpriteColor(c), Color.white, 0.2f);
        AudioManager.instance.PlayOneShot("Damage");

        ShowDamageText(damage);
    }

    private void ShowDamageText(int damage)
    {
        // �ؽ�Ʈ ����
        damageText.gameObject.SetActive(true);
        damageText.text = damage.ToString();

        // ���� ��ġ (ĳ���� �Ӹ� ��)
        Vector3 startPos = transform.position + new Vector3(0, 0.8f, 0);
        damageText.transform.position = startPos;

        // �ʱ� ����
        damageText.alpha = 1f;

        // DOTween���� �ִϸ��̼�
        Sequence seq = DOTween.Sequence();
        seq.Append(damageText.transform.DOMoveY(startPos.y + 0.5f, 1.0f)); // ���� �̵�
        seq.Join(damageText.DOFade(0f, 0.5f)); // ���ÿ� ���̵�ƿ�
        seq.OnComplete(() => damageText.gameObject.SetActive(false)); // ������ �ؽ�Ʈ ����
    }

    public void OnWarningNotification(long expectedAt)
    {
        var remain = (expectedAt - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000.0f;
        bombRemainTime = Mathf.Max(0, (float)remain);
        bombTimerText.gameObject.SetActive(true); // ǥ�� Ȱ��ȭ
    }
    public void SetBombIcon(bool hasBomb)
    {
            bombIcon.SetActive(hasBomb);
    }
}