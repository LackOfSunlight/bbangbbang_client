using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupBattle : UIListBase<Card>
{
    [SerializeField] private UIPagingViewController uiPagingViewController;
    [SerializeField] private GameObject select;
    [SerializeField] private Button useButton;
    [SerializeField] private Button takeButton;
    [SerializeField] private Transform useCardSlot;
    [SerializeField] private TMP_Text title;
    [SerializeField] private GameObject nonCardText;
    [SerializeField] private TMP_Text timer;

    float time = 0;

    CardDataSO targetCard;
    UserInfo targetUser;
    List<CardDataSO> cards;
    Action<int, long> callback;

    private Coroutine timerCoroutine;

    public override void Opened(object[] param)
    {
        targetCard = DataManager.instance.GetData<CardDataSO>(param[0].ToString());
        targetUser = DataManager.instance.users.Find(obj => obj.id == (long)param[1]);
        if (SocketManager.instance.isConnected)
        {
            callback = (Action<int, long>)param[2];
        }
        SetList();
        uiPagingViewController.OnChangeValue += OnChangeValue;
        uiPagingViewController.OnMoveStart += OnMoveStart;
        uiPagingViewController.OnMoveEnd += OnMoveEnd;
        title.text = targetCard.displayName;
        AddUseCard(targetCard);

        if(timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        SetUserSelectTurn(targetUser.characterData.StateInfo.NextStateAt);
    }

    public void SetActiveControl(bool isActive)
    {
        useButton.interactable = isActive;
        takeButton.interactable = isActive;
    }

    public void OnChangeValue(int idx)
    {
        useButton.interactable = UserInfo.myInfo.handCards[idx].isUsable;
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupBattle>();
        StopAllCoroutines(); // UI 닫을 때 코루틴 중단
    }

    public void AddUseCard(CardDataSO data)
    {
        var item = Instantiate(itemPrefab, useCardSlot);
        item.Init(data);
    }

    public override void SetList()
    {
        ClearList();
        cards = UserInfo.myInfo.handCards.FindAll(obj => obj.rcode == targetCard.defCard);
        foreach (var data in cards)
        {
            var item = AddItem();
            item.Init(data, OnClickItem);
        }
        select.SetActive(items.Count > 0);
        nonCardText.SetActive(items.Count == 0);
    }

    public void OnClickItem(CardDataSO data)
    {
        
    }

    private void OnMoveStart()
    {
        select.SetActive(false);
        useButton.interactable = false;
    }

    private void OnMoveEnd()
    {
        select.SetActive(true);
        useButton.interactable = true;
    }

    public void OnClickUse()
    {
        var idx = uiPagingViewController.selectedIdx;
        var card = cards[idx];
        if (SocketManager.instance.isConnected)
        {
            callback.Invoke((int)card.cardType, targetUser.id);
        }
        else
        {
            UserInfo.myInfo.OnUseCard(card);
            GameManager.instance.OnSelectCard(UserInfo.myInfo, card.rcode, targetUser, targetCard.rcode);
            AddUseCard(card);
            SetList();
        }

        AudioManager.instance.PlayOneShot("Button");

    }

    public void OnClickDamage()
    {
        if (SocketManager.instance.isConnected)
        {
            callback.Invoke(0, 0);
        }
        HideDirect();
        AudioManager.instance.PlayOneShot("Button");
    }

    public void SetUserSelectTurn(long nextTimeAt)
    {
        Debug.Log($"서버에서 받은 시간: {targetUser.characterData.StateInfo.NextStateAt}");
        long clientNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        long offset = clientNow - (nextTimeAt - 5000);

        //this.time = time;
        //timer.text = time.ToString();
        timerCoroutine = StartCoroutine(SetTimer(nextTimeAt, offset));
    }

    IEnumerator SetTimer(long nextTimeAt,long offset)
    {      
        while (true)
        {
            // 클라 현재 시간 (ms)
            long clientNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long serverNowEstimate = clientNow - offset;
            long remainMs = nextTimeAt - serverNowEstimate;

            if (remainMs <= 0)
            {
                timer.text = "0";
                OnClickDamage();
                yield break; // 코루틴 종료
            }

            int remainSec = Mathf.CeilToInt(remainMs / 1000f);
            timer.text = remainSec.ToString();

            yield return null; // 다음 프레임까지 대기

        }
    }

}