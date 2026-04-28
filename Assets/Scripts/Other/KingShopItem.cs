using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class KingShopItem : MonoBehaviour
{
    public TextMeshProUGUI priceTxt;
    public int price;
    public float id;
    private void Start()
    {
        updateItem();
    }
    private void updateItem()
    {
        switch (id)
        {
            case 1:
                if (DataManager.Instance.data.hasWateringCan)
                {
                    priceTxt.text = "ÒÑ»ñµÃ";
                }
                else priceTxt.text = price.ToString();
                break;
        }

    }
    public void buyItem()
    {
        if (!checkIfCanBuy()) return;

        if(DataManager.Instance.data.coinCount >= price)
        {
            buyEvent();
        }
        else
        {
            DialogKingShop.Instance.cantAfford();
        }
    }
    private bool checkIfCanBuy()
    {
        switch (id)
        {
            case 1:
                if (DataManager.Instance.data.hasWateringCan)
                {
                    return false;
                }
                break;
        }
        return true;
    }
    private void buyEvent()
    {
        switch (id)
        {
            case 1:
                DataManager.Instance.data.hasWateringCan = true;
                HandManager.Instance.wateringCan.gameObject.SetActive(true);
                break;
        }
        CoinManager.Instance.changeCoin(price * -1);
        DialogKingShop.Instance.coinCount.text = DataManager.Instance.data.coinCount.ToString();
        updateItem();
    }
}
