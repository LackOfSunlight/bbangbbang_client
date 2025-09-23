using DG.Tweening;
using Ironcow;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupCharacteristic : UIBase
{
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float closeDuration = 0.2f;

    [SerializeField] private GameObject contents;
    [SerializeField] private CharacteristicInfo characteristicInfoPrefab;
    public List<Sprite> images;


    private string[] characterNames = new string[9]
    {
        "빨강이",
        "상어군",
        "말랑이",
        "개굴군",
        "핑크군",
        "물안경군",
        "가면군",
        "공룡이",
        "팡크슬라임"
    };

    private string[] characterDescriptions = new string[9]
    {
        "하루에 원하는 만큼 빵야!를 사용할 수 있다.",
        "빵야!를 막기 위해 쉴드 2개가 필요함",
        "생명력을 1 잃을 때마다 카드 한장을 획득한다.",
        "표적이 될 때 25% 확률로 공격을 막는다.",
        "남은 카드가 없으면 새로 카드를 한장 받는다.",
        "추가로 두명의 위치가 미니맵에 표시 됨 (최대 4명)",
        "다른 사람이 사망 시 모든 카드를 손에 가져온다.",
        "다른 유저에게서 미니맵 상 위치를 감춤",
        "피격 시 가해자의 카드를 한장 가져옴."
    };

    public override async void Opened(object[] param)
    {
        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }


        // 캐릭터 데이터 생성
        for (int i = 0; i < characterNames.Length; i++)
        {
            // 프리팹 인스턴스 생성
            var obj = Instantiate(characteristicInfoPrefab, contents.transform);

            // 데이터 세팅
            obj.Setting(images[i], characterNames[i], characterDescriptions[i]);
        }
        this.gameObject.SetActive(true);


        // 시작 크기 0으로 초기화
        transform.localScale = Vector3.zero;

        // 0 → 1로 커지면서 "튀어나오는" 느낌 (OutBack으로 튕기는 효과)
        transform.DOScale(Vector3.one, openDuration).SetEase(Ease.OutBack);
    }
    public void OnClickOverlay()
    {
        CloseWithAnim();
    }

    public async Task CloseWithAnim()
    {
        await transform
            .DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .AsyncWaitForCompletion();

        HideDirect();
    }


    public override void HideDirect()
    {

        UIManager.Hide<PopupCharacteristic>();
    }

}
