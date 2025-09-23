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
        "������",
        "��",
        "������",
        "������",
        "��ũ��",
        "���Ȱ決",
        "���鱺",
        "������",
        "��ũ������"
    };

    private string[] characterDescriptions = new string[9]
    {
        "�Ϸ翡 ���ϴ� ��ŭ ����!�� ����� �� �ִ�.",
        "����!�� ���� ���� ���� 2���� �ʿ���",
        "������� 1 ���� ������ ī�� ������ ȹ���Ѵ�.",
        "ǥ���� �� �� 25% Ȯ���� ������ ���´�.",
        "���� ī�尡 ������ ���� ī�带 ���� �޴´�.",
        "�߰��� �θ��� ��ġ�� �̴ϸʿ� ǥ�� �� (�ִ� 4��)",
        "�ٸ� ����� ��� �� ��� ī�带 �տ� �����´�.",
        "�ٸ� �������Լ� �̴ϸ� �� ��ġ�� ����",
        "�ǰ� �� �������� ī�带 ���� ������."
    };

    public override async void Opened(object[] param)
    {
        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }


        // ĳ���� ������ ����
        for (int i = 0; i < characterNames.Length; i++)
        {
            // ������ �ν��Ͻ� ����
            var obj = Instantiate(characteristicInfoPrefab, contents.transform);

            // ������ ����
            obj.Setting(images[i], characterNames[i], characterDescriptions[i]);
        }
        this.gameObject.SetActive(true);


        // ���� ũ�� 0���� �ʱ�ȭ
        transform.localScale = Vector3.zero;

        // 0 �� 1�� Ŀ���鼭 "Ƣ�����" ���� (OutBack���� ƨ��� ȿ��)
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
