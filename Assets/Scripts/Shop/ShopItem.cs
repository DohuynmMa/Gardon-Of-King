using UnityEngine;
using Assets.Scripts.Utils;
public class ShopItem : MonoBehaviour
{
    public int id;//商品id
    public int price;//购买价格
    public int count;//数量

    public void openInfo()
    {
        DialogMainShop.Instance.openShopItemInfo(id);
    }

    /// <summary>
    /// 点击购买后发生的事
    /// </summary>
    public virtual bool buy()
    {
        int coin = DataManager.Instance.data.coinCount;
        if (coin >= price)
        {
            getItem();
            return true;
        }
        else
        {
            DialogMainShop.Instance.cantAfford();
            return false;
        }
    }
    public virtual void getItem()
    {

    }
    public virtual void onShow()
    {

    }
    public virtual void onHide()
    {

    }
}
