using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;

public class NewCard : Item
{
    public string itemTag;
    public Card addCardPrefeb;
    public bool choujiang;//是否抽奖结束?
    void Start()
    {
        float flyRange = Random.Range(1f, 2f);
        float flyHeight = Random.Range(1f, 2f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x - flyRange, transform.position.y + flyHeight, 0), new Vector3(transform.position.x - flyRange * 2, transform.position.y - flyHeight, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    public void addCard()
    {
        if (addCardPrefeb == null) return;
        //判断新手,删除试用卡,替换正式卡
        if(DataManager.Instance.data.cardCount < 8)
        {
            DataManager.Instance.data.cardList.Remove(DataManager.Instance.data.cardList[2]);
            DataManager.Instance.data.cardList.Add(addCardPrefeb);
        }
        //正常情况添加卡
        else
        {
            if (DataManager.Instance.data.cardList.Count < 8)
            {
                if (!DataManager.Instance.data.cardList.Contains(addCardPrefeb) && !DataManager.Instance.data.cardBag.Contains(addCardPrefeb))
                {
                    DataManager.Instance.data.cardList.Add(addCardPrefeb);
                }
            }
            else
            {
                if (!DataManager.Instance.data.cardList.Contains(addCardPrefeb) && !DataManager.Instance.data.cardBag.Contains(addCardPrefeb))
                {
                    DataManager.Instance.data.cardBag.Add(addCardPrefeb);
                }
            }
        }
        if (itemTag == "C1")
        {
            DataManager.Instance.data.timeC1L1++;
        }
        else if (itemTag == "C2")
        {
            DataManager.Instance.data.timeC1L2++;
        }
        else if (itemTag == "C3")
        {
            DataManager.Instance.data.timeC1L3++;
        }
        else if (itemTag == "C11")
        {
            DataManager.Instance.data.timeC1L5++;
        }
        else if (itemTag == "C12")
        {
            DataManager.Instance.data.timeC1L6++;
        }
        else if (itemTag == "C13")
        {
            DataManager.Instance.data.timeC1L7++;
        }
        else if (itemTag == "C27")
        {
            DataManager.Instance.data.timeC2L1++;
        }
        else if (itemTag == "C28")
        {
            DataManager.Instance.data.timeC2L2++;
        }
        else if (itemTag == "C32")
        {
            DataManager.Instance.data.timeC2L3++;
        }
        else if (itemTag == "C33")
        {
            DataManager.Instance.data.timeC2L4++;
        }
        DataManager.Instance.data.cardCount++;


    }
    public override void clickEvent()
    {
        addCard();
        if (choujiang)
        {
            DialogMainShop.Instance.showShopItem();
        }
        AchievementManager.Instance.hasDropItem = false;
        base.clickEvent();
    }
    public override void destroyItem()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.SpreadingStar), transform.position, Quaternion.identity);
        DataManager.Instance.savePlayerData();
        Destroy(gameObject);
    }
}
