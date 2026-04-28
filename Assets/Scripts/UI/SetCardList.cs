using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utils;

public class SetCardList : MonoBehaviour
{
    public Vector3 currentPos;
    public int index;
    public RectTransform rectTransform;
    private Card card;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        card = GetComponent<Card>();
    }
    public void movePos()
    {
        if (card == null) return;
        if (hasCardInCardList(card))
        {
            float temp = Mathf.Floor(index / 4);
            currentPos.x = -552 + (index - temp * 4) * 58; currentPos.y = 164 - temp * 79;
            rectTransform.DOAnchorPos(currentPos, 0.3f).SetEase(Ease.OutSine).OnComplete(() => {
            });
        }
        else if (hasCardInBag(card))
        {
            float temp = Mathf.Floor(index / 8);
            currentPos.x = -192 + (index - temp * 8) * 55;currentPos.y = 164 - temp * 70;
            rectTransform.DOAnchorPos(currentPos, 0.3f).SetEase(Ease.OutSine).OnComplete(()=> {
            });
        }
    }
    public void movePosTower(List<Card> totalCardList)
    {
        if (card == null) return;
        foreach (Card card1 in totalCardList)
        {
            if (card.cardTag == card1.cardTag)
            {
                index = totalCardList.IndexOf(card1);
            }
        }
        float temp = Mathf.Floor(index / 8);
        currentPos.x = -192 + (index - temp * 8) * 55; currentPos.y = 164 - temp * 70;
        rectTransform.DOAnchorPos(currentPos, 0.3f).SetEase(Ease.OutSine).OnComplete(() => {
        });
    }
    public void movePosFirst()
    {
        if (card == null) return;
        if (hasCardInCardList(card))
        {
            float temp = Mathf.Floor(index / 4);
            currentPos.x = -552 + (index - temp * 4) * 58; currentPos.y = 164 - temp * 79;
            rectTransform.localPosition = currentPos;
        }
        else if (hasCardInBag(card))
        {
            float temp = Mathf.Floor(index / 8);
            currentPos.x = -192 + (index - temp * 8) * 55; currentPos.y = 164 - temp * 70;
            rectTransform.localPosition = currentPos;
        }
        rectTransform.localScale = new Vector3(0.34f, 0.48f, 0f);//UI  ≈‰
    }
    public void selectCard()
    {
        if (card == null || Utils.scene != "GameMenu") return;
        switch (DialogPFW.Instance.setCardUiTemp)
        {
            case 0:
                transform.SetAsLastSibling();
                Sounds.tap.play();
                if (hasCardInCardList(card))
                {
                    Card card = findCardInCardList(this.card);
                    CardManager.Instance.removeCardFromList(card);
                }
                else if (hasCardInBag(card))
                {
                    if (CardManager.Instance.cardList.Count >= 8) return;
                    Card card = findCardInBag(this.card);
                    CardManager.Instance.addCardToList(card);
                }
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    transform.SetAsFirstSibling();
                });
                CardManager.Instance.washCard();
                break;
            case 1:
                //todo …Ë÷√Entity
                Sounds.tap.play();
                DataManager.Instance.data.towerEntity = GetComponent<Card>().entityType;
                DialogPFWTower.Instance.updateTowerEntity();
                break;
        }
        
    }
    public bool hasCardInBag(Card card)
    {
        foreach(Card card1 in DataManager.Instance.data.cardBag)
        {
            if (card.cardTag == card1.cardTag)
            {
                index = DataManager.Instance.data.cardBag.IndexOf(card1);
                return true;
            }
        }
        return false;
    }
    public bool hasCardInCardList(Card card)
    {
        foreach (Card card1 in CardManager.Instance.cardList)
        {
            if (card.cardTag == card1.cardTag)
            {
                index = CardManager.Instance.cardList.IndexOf(card1);
                return true;
            }
        }
        return false;
    }
    public Card findCardInBag(Card card)
    {
        foreach (Card card1 in DataManager.Instance.data.cardBag)
        {
            if (card.cardTag == card1.cardTag)
            {
                return card1;
            }
        }
        return null;
    }
    public Card findCardInCardList(Card card)
    {
        foreach (Card card1 in CardManager.Instance.cardList)
        {
            if (card.cardTag == card1.cardTag)
            {
                return card1;
            }
        }
        return null;
    }
}
