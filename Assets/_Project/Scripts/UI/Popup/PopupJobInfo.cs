using DG.Tweening;
using Ironcow;
using UnityEngine;
using System.Threading.Tasks;

public class PopupJobInfo : UIBase
{
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float closeDuration = 0.2f;

    public override async void Opened(object[] param)
    { 
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


    public override void HideDirect() { 

        UIManager.Hide<PopupJobInfo>();
    }

}
