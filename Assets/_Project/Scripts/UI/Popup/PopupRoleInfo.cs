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

    private string[] roleTexts = new string[5] { "민간인 ", "타켓", "보디가드", "히트맨", "싸이코패스" };
    private string[] roleDescriptionTexts = new string[5] {
        "특별한 역할이 없습니다. 살아남으세요.",
        "히트맨과 싸이코패스를 죽이고 승리하세요." ,
        "타켓을 보호하여 팀을 승리로 이끄세요." ,
        "타켓을 제거하여 승리하세요." ,
        "당신을 제외한 모든 참가자를 죽이고 승리하세요."
    };

    private string[] characterDescriptions = new string[14]
    {
        "능력없음",
        "하루에 원하는 만큼 빵야!를 사용할 수 있다.",
        "빵야!를 쉴드로, 쉴드를 빵야!로 사용할 수 있다.",
        "빵야!를 막기 위해 쉴드 2개가 필요함",
        "카드를 받을 때 3장중 두장을 선택한다.",
        "생명력을 1 잃을 때마다 카드 한장을 획득한다.",
        "카드 두장을 버려 체력을 1 회복 한다.",
        "표적이 될 때 25% 확률로 공격을 막는다.",
        "남은 카드가 없으면 새로 카드를 한장 받는다.",
        "추가로 두명의 위치가 미니맵에 표시 됨 (최대 4명)",
        "다른 사람이 사망 시 모든 카드를 손에 가져온다.",
        "카드를 가져올 때 다른 유저의 카드를 가져올 수 있음.",
        "다른 유저에게서 미니맵 상 위치를 감춤",
        "피격 시 가해자의 카드를 한장 가져옴."
    };

    private string[] characterNames = new string[14]
    {
        "이름없음",
        "빨강이",
        "파랄이",
        "상어군",
        "기사군",
        "말랑이",
        "다이노",
        "개굴군",
        "핑크군",
        "물안경군",
        "가면군",
        "슬라임",
        "공룡이",
        "팡크슬라임"
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


        // 시작 크기 0으로 초기화
        transform.localScale = Vector3.zero;

        // 0 → 1로 커지면서 "튀어나오는" 느낌 (OutBack으로 튕기는 효과)
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupRoleInfo>();
    }

    public async Task CloseWithFade()
    {
        // Content만 페이드아웃
        await canvasGroup.DOFade(0f, 0.5f).AsyncWaitForCompletion();

        HideDirect();
    }
}
