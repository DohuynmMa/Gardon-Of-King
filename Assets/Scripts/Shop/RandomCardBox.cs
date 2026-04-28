using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
using TMPro;
using System;
public class RandomCardBox : ShopItem
{
    public List<NewCard> cards;
    public TextMeshProUGUI priceTxtUi;
    public override void getItem()
    {
        var playVideo = !DataManager.Instance.data.skipVideo;
        var mainShop = DialogMainShop.Instance;
        if (playVideo)VideoManager.Instance.play(0);
        mainShop.shopUI.SetActive(false);
        mainShop.hideShopItem();
        mainShop.closeShopItemInfo();
        DOVirtual.DelayedCall(playVideo ? VideoManager.Instance.getTime(0) : 1f, () =>
        {
            mainShop.shopUI.SetActive(true);
            openBox();
        });
    }
    public override bool buy()
    {
        if (hasAllCard())
        {
            Utils.showDialog("已集齐宝箱中的所有卡牌", 18.1f, Color.white, "确 定", 20f, Color.white);
            return false;
        }
        return base.buy();
    }
    private void openBox()
    {
        var gift = Instantiate(summonCard().gameObject,Vector3.zero,Quaternion.identity);
        CoinManager.Instance.changeCoin(price * -1);
    }
    private NewCard summonCard()
    {
        var card = cards[UnityEngine.Random.Range(0, cards.Count)];
        if (CardManager.Instance.hasCard(card.addCardPrefeb.entityType))
        {
            return summonCard();
        }
        else return card;
    }
    public bool hasAllCard()
    {
        foreach(var card in cards)
        {
            if (!CardManager.Instance.hasCard(card.addCardPrefeb.entityType)) return false;
        }
        return true;
    }
    public int hasCardCount()
    {
        var count = 0;
        foreach (var card in cards)
        {
            if (CardManager.Instance.hasCard(card.addCardPrefeb.entityType)) count++;
        }
        return count;
    }
    public override void onShow()
    {
        price = 200 * (hasCardCount() + 1);
        priceTxtUi.text = hasAllCard() ? "已集齐" : price.ToString();

        //更新颜色 防止看不清
        if ((DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 6) || DateTime.Now.Hour >= 18)
        {
            priceTxtUi.color = Color.white;
        }
        else
        {
            priceTxtUi.color = Color.black;
        }

        base.onShow();
    }
}
