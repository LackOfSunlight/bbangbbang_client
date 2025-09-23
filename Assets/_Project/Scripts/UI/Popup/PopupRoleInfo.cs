using DG.Tweening;
using Ironcow;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;


[Serializable]
public class RoleInfo
{
    public GameObject root;
    public TMP_Text nickname;
    public Image character;
}
public class PopupRoleInfo : UIBase
{
    [SerializeField] private Image role;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text roleDescription;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterDescription;
    [SerializeField] private TMP_Text characterName;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private string[] roleTexts = new string[5] { "�ΰ��� ", "Ÿ��", "���𰡵�", "��Ʈ��", "�������н�" };
    private string[] roleDescriptionTexts = new string[5] {
        "Ư���� ������ �����ϴ�. ��Ƴ�������.",
        "��Ʈ�ǰ� �������н��� ���̰� �¸��ϼ���." ,
        "Ÿ���� ��ȣ�Ͽ� ���� �¸��� �̲�����." ,
        "Ÿ���� �����Ͽ� �¸��ϼ���." ,
        "����� ������ ��� �����ڸ� ���̰� �¸��ϼ���."
    };

    private string[] characterDescriptions = new string[14]
    {
        "�ɷ¾���",
        "�Ϸ翡 ���ϴ� ��ŭ ����!�� ����� �� �ִ�.",
        "����!�� �����, ���带 ����!�� ����� �� �ִ�.",
        "����!�� ���� ���� ���� 2���� �ʿ���",
        "ī�带 ���� �� 3���� ������ �����Ѵ�.",
        "������� 1 ���� ������ ī�� ������ ȹ���Ѵ�.",
        "ī�� ������ ���� ü���� 1 ȸ�� �Ѵ�.",
        "ǥ���� �� �� 25% Ȯ���� ������ ���´�.",
        "���� ī�尡 ������ ���� ī�带 ���� �޴´�.",
        "�߰��� �θ��� ��ġ�� �̴ϸʿ� ǥ�� �� (�ִ� 4��)",
        "�ٸ� ����� ��� �� ��� ī�带 �տ� �����´�.",
        "ī�带 ������ �� �ٸ� ������ ī�带 ������ �� ����.",
        "�ٸ� �������Լ� �̴ϸ� �� ��ġ�� ����",
        "�ǰ� �� �������� ī�带 ���� ������."
    };

    private string[] characterNames = new string[14]
    {
        "�̸�����",
        "������",
        "�Ķ���",
        "��",
        "��籺",
        "������",
        "���̳�",
        "������",
        "��ũ��",
        "���Ȱ決",
        "���鱺",
        "������",
        "������",
        "��ũ������"
    };

    public override async void Opened(object[] param)
    {
        var roleType = UserInfo.myInfo.roleType;
        var characterType = UserInfo.myInfo.characterData.CharacterType;
        var rcode = UserInfo.myInfo.selectedCharacterRcode;

        role.gameObject.SetActive(true);
        roleText.gameObject.SetActive(true);
        roleDescription.gameObject.SetActive(true);

        role.sprite = await ResourceManager.instance.LoadAsset<Sprite>("Role_" + roleType.ToString(), eAddressableType.Thumbnail);
        characterImage.sprite = await ResourceManager.instance.LoadAsset<Sprite>(rcode, eAddressableType.Thumbnail);
        characterName.text = characterNames[(int)characterType];
        characterDescription.text = characterDescriptions[(int)characterType];
        roleText.text = roleTexts[(int)roleType];
        roleDescription.text = roleDescriptionTexts[(int)roleType];


        // ���� ũ�� 0���� �ʱ�ȭ
        transform.localScale = Vector3.zero;

        // 0 �� 1�� Ŀ���鼭 "Ƣ�����" ���� (OutBack���� ƨ��� ȿ��)
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupRoleInfo>();
    }

    public async Task CloseWithFade()
    {
        // Content�� ���̵�ƿ�
        await canvasGroup.DOFade(0f, 0.5f).AsyncWaitForCompletion();

        HideDirect();
    }
}
