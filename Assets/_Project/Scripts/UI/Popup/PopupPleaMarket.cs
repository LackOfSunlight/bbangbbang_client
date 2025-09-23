using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Google.Protobuf.Collections;
using System;

public class PopupPleaMarket : UIBase
{
    [SerializeField] private Transform grid;
    [SerializeField] private List<GameObject> cardObjects;
    [SerializeField] private List<TMP_Text> nameTexts;
    [SerializeField] private TMP_Text timer;

    List<Card> cards = new List<Card>();
    long id;
    float time = 0;
    public bool isMyTurn;

    public bool isInitCards { get => cards.Count > 0; }
    public override async void Opened(object[] param)
    {

    }

    public async void Init(long id)
    {
        this.id = id;
        GameManager.instance.SetPleaMarketCards();
        cardObjects.ForEach(obj => obj.SetActive(false));
        for (int i = 0; i < GameManager.instance.pleaMarketCards.Count; i++)
        {
            var cardData = GameManager.instance.pleaMarketCards[i];
            var card = Instantiate(await ResourceManager.instance.LoadAsset<Card>("Card", eAddressableType.Prefabs), grid);
            card.Init(cardData, i, (id == UserInfo.myInfo.id ? OnClickItem : null));
            cards.Add(card);
            cardObjects[i].SetActive(true);
        }
    }

    public void OnClickItem(int idx)
    {
        if (!isMyTurn) return;
        if (SocketManager.instance.isConnected)
        {
            GamePacket packet = new GamePacket();
            packet.FleaMarketPickRequest = new C2SFleaMarketPickRequest() { PickIndex = idx };
            SocketManager.instance.Send(packet);
            StopAllCoroutines();
            timer.text = "";
        }
        else
        {
            GameManager.instance.OnSelectCard(UserInfo.myInfo, cards[idx].cardData.rcode, DataManager.instance.users.Find(obj => obj.id == id), "CAD00010");
        }
    }

    public void SetNextUserTurn(UserInfo userinfo, int selectedCardIdx)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Init(GameManager.instance.pleaMarketCards[i], i, userinfo.id == UserInfo.myInfo.id ? OnClickItem : null);
        }
        SetNameText(selectedCardIdx, DataManager.instance.users.Find(obj => obj.id == id));
        id = userinfo.id;
    }

    public void SetNameText(int idx, UserInfo userinfo)
    {
        nameTexts[idx].text = userinfo.nickname;
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupPleaMarket>();
        StopAllCoroutines(); // UI 닫을 때 코루틴 중단
    }

    public async void SetCards(RepeatedField<CardType> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            var cardData = cards[i].GetCardData();
            var card = Instantiate(await ResourceManager.instance.LoadAsset<Card>("Card", eAddressableType.Prefabs), grid);
            card.Init(cardData, i, OnClickItem);
            this.cards.Add(card);
        }
    }

    public void OnSelectedCard(RepeatedField<int> idxs)
    {
        for(int i = 0; i < idxs.Count; i++)
        {
            this.cards[idxs[i]].SetActive(false);
        }
    }

    public void SetUserSelectTurn(long nextTimeAt)
    {
        isMyTurn = true;

        long clientNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        long offset = clientNow - (nextTimeAt - 10000);

        if (nextTimeAt == 0)
        {
            nextTimeAt = 5;
        }
        //this.time = nextTimeAt;
        //timer.text = nextTimeAt.ToString();
        StartCoroutine(SetTimer(nextTimeAt, offset));
    }

    public void SetUserSelectNotTurn()
    {
        isMyTurn = false;
        timer.text = "";
    }

    IEnumerator SetTimer(long nextTimeAt, long offset)
    {
        while (true)
        {
            // 클라 현재 시간 (ms)
            long clientNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long serverNowEstimate = clientNow - offset;
            long remainMs = nextTimeAt - serverNowEstimate;

            if (remainMs <= 0)
            {
                timer.text = "";
                OnClickItem(cards.FindIndex(obj => obj.gameObject.activeInHierarchy));
                yield break; // 코루틴 종료
            }

            int remainSec = Mathf.CeilToInt(remainMs / 1000f);
            timer.text = remainSec.ToString();

            yield return null; // 다음 프레임까지 대기

        }
      
    }
}