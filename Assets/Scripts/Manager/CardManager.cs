using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Assets.Scripts.Utils;
public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public List<Card> cardList;
    public List<Card> cardEnable;
    public List<Card> cardWaiting;
    public List<Card> battleCardList;
    private void Start()
    {
        loadCardData();
    }
    public static Card getCardByTag(string cardTag)
    {
        foreach (var card in BridgeManager.Instance.allCards)
        {
            if (card.cardTag == cardTag)
            {
                return card;
            }
        }
        Debug.Log("找不到卡片 " + cardTag);
        return null;
    }
    //游戏外设置
    private void loadCardData()
    {
        DOVirtual.DelayedCall(0.2f, () =>
        {
            cardList = DataManager.Instance.data.cardList;
        });
    }
    public void setCard()
    {
        if (Utils.scene != "GameMenu") return;
        foreach (var card in DataManager.Instance.data.cardBag)
        {
            var currentCard = Instantiate(card,transform.position,Quaternion.identity);
            currentCard.TransitionsToReady();
            currentCard.transform.SetParent(DialogPFW.Instance.cardBagUI.transform);
            currentCard.GetComponent<SetCardList>().movePosFirst();
            currentCard.GetComponent<RectTransform>().localScale *= 1.4f;
        }
        foreach (var card in cardList)
        {
            var currentCard = Instantiate(card, transform.position, Quaternion.identity);
            currentCard.TransitionsToReady();
            currentCard.transform.SetParent(DialogPFW.Instance.cardBagUI.transform);
            currentCard.GetComponent<SetCardList>().movePosFirst();
            currentCard.GetComponent<RectTransform>().localScale *= 1.4f;
        }
        checkCard();
    }
    public void setTowerCard(List<Card> totalCardList)
    {
        if (Utils.scene != "GameMenu") return;
        foreach (var card in totalCardList)
        {
            var currentCard = Instantiate(card, transform.position, Quaternion.identity);
            currentCard.TransitionsToReady();
            currentCard.transform.SetParent(DialogPFW.Instance.cardBagUI.transform);
            currentCard.GetComponent<SetCardList>().movePosFirst();
            currentCard.GetComponent<RectTransform>().localScale *= 1.4f;
        }
        washTowerCard(totalCardList);
    }
    public void stopSetCard()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var card in cards)
        {
            Destroy(card);
        }
    }
    public void washCard()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var card in cards)
        {
            card.GetComponent<SetCardList>().movePos();
        }
    }
    public void washTowerCard(List<Card> totalCardList)
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var card in cards)
        {
            card.GetComponent<SetCardList>().movePosTower(totalCardList);
        }
    }
    public void checkCard()
    {
        cardList = new HashSet<Card>(cardList).ToList();
        DataManager.Instance.data.cardBag = new HashSet<Card>(DataManager.Instance.data.cardBag).ToList();
        bool changed = false;
        if(cardList.Count != 8)
        {
            DataManager.Instance.data.cardBag.AddRange(cardList);
            cardList.Clear();
            var count = Math.Min(8, DataManager.Instance.data.cardBag.Count);
            cardList.AddRange(DataManager.Instance.data.cardBag.Take(count));
            DataManager.Instance.data.cardBag.RemoveRange(0, count);
            changed = true;
        }
        foreach (var card in cardList)
        {
            for (var i = DataManager.Instance.data.cardBag.Count - 1; i >= 0; i--)
            {
                var card1 = DataManager.Instance.data.cardBag[i];
                if (card1.cardTag == card.cardTag)
                {
                    DataManager.Instance.data.cardBag.Remove(card);
                    changed = true;
                }
            }
        }
        if (changed)
        {
            washCard();
            DataManager.Instance.data.cardList = cardList;
            DataManager.Instance.savePlayerData();
        }
    }
    public void addCardToList(Card card)
    {
        if (DataManager.Instance.data.cardBag.Contains(card) && !cardList.Contains(card))
        {
            cardList.Add(card);
            DataManager.Instance.data.cardList = cardList;
            DataManager.Instance.data.cardBag.Remove(card);
        }
        else print("failed");
    }
    public void removeCardFromList(Card card)
    {
        if (cardList.Contains(card))
        {
            cardList.Remove(card);
            DataManager.Instance.data.cardList = cardList;
            DataManager.Instance.data.cardBag.Add(card);
        }
        else print("failed");
    }
    public bool onCardList(Card card)
    {
        if (cardList.Contains(card))
        {
            return true;
        }
        else return false;
    }
    //游戏内设置
    public void gameStart(List<Card> newCardList = null)
    {
        cardEnable.Clear();
        cardWaiting.Clear();
        battleCardList.Clear();
        cardList = newCardList == null ? DataManager.Instance.data.cardList : newCardList;
        for (var i = 0; i < cardList.Count; i++)
        {
            var card = cardList[i];
            var currentCard = Instantiate(card,transform.position,Quaternion.identity);
            currentCard.transform.SetParent(DialogGamingMenu.Instance.cardAreaUI.transform);
            battleCardList.Add(currentCard);
            currentCard.GetComponent<RectTransform>().localScale = new Vector3(0.65f, 0.94f, 0.82f);
        }
        var cardOrder = 5;
        for (var i = 0; i < 4; i++)
        {
            var temp = UnityEngine.Random.Range(0, battleCardList.Count);
            if (cardEnable.Contains(battleCardList[temp]))
            {
                i--;
            }
            else
            {
                battleCardList[temp].cardOrder = cardOrder;
                switch (cardOrder)
                {
                    case 5:
                        battleCardList[temp].GetComponent<RectTransform>().localPosition = new Vector3(-819.9f, 483.7f, 10);
                        break;
                    case 6:
                        battleCardList[temp].GetComponent<RectTransform>().localPosition = new Vector3(-744.57f, 483.7f, 10);
                        break;
                    case 7:
                        battleCardList[temp].GetComponent<RectTransform>().localPosition = new Vector3(-668.2f, 483.7f, 10);
                        break;
                    case 8:
                        battleCardList[temp].GetComponent<RectTransform>().localPosition = new Vector3(-594.3f, 483.7f, 10);
                        break;
                }
                cardOrder++;
                cardEnable.Add(battleCardList[temp]);
            }
        }
        var cardWaitingTemp = battleCardList.Except(cardEnable).ToList();
        cardOrder = 1;
        cardWaiting = cardWaitingTemp.ToList();
        for (var i = 0; i < cardWaitingTemp.Count; i++)
        {
            var card = cardWaitingTemp[i];
            hideCard(card, false);
            card.cardOrder = cardOrder;
            cardWaiting[4 - cardOrder] = card;
            cardOrder++;
        }
        aboutToTransitionToEnable(cardWaiting[0]);
    }
    public void transitionToWaiting(Card card)
    {
        if (card == null) return;
        cardWaiting.Add(card);
        cardEnable.Remove(card);
        hideCard(card, false);
        allCardOrderChange(card);
    }
    public void transitionToEnable(Card card,Card lastCard)
    {
        if (card == null || lastCard == null) return;
        cardWaiting.Remove(card);
        cardEnable.Add(card);
        card.GetComponent<RectTransform>().localPosition = lastCard.GetComponent<RectTransform>().localPosition;
    }
    public void aboutToTransitionToEnable(Card card)
    {
        if (card == null) return;
        hideCard(card, true);
        card.GetComponent<RectTransform>().localPosition = new Vector3(-476, 477f, 0);
    }
    public void allCardOrderChange(Card usedCard)
    {
        transitionToEnable(cardWaiting[0], usedCard);
        aboutToTransitionToEnable(cardWaiting[0]);
    }
    public void hideCard(Card card,bool ifHide)
    {
        if (card == null) return;
        card.cardLight.GetComponent<Image>().enabled = ifHide;
        card.cardGray.GetComponent<Image>().enabled = ifHide;
        card.cardMask.GetComponent<Image>().enabled = ifHide;
    }
    public bool hasCard(EntityType entityType)
    {
        var allCards = new List<Card>(DataManager.Instance.data.cardList);
        allCards.AddRange(DataManager.Instance.data.cardBag);
        foreach (var card in allCards)
        {
            if (card.entityType == entityType) return true;
        }
        return false;
    }
}
