using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public List<Item> coinPrefebs;
    public static CoinManager Instance { get; private set; }
    public DialogWallet wallet { get => DialogWallet.Instance; }
    private void Awake()
    {
        Instance = this;
    }
    public void setCoin(int count)
    {
        wallet.coinUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(726, -483), 1f);
        DOVirtual.DelayedCall(3f, () =>
        {
            wallet.hideWallet();
        });
        DataManager.Instance.data.coinCount = count;
        wallet.coinCount.text = DataManager.Instance.data.coinCount.ToString();
        DataManager.Instance.savePlayerData();
    }
    public void changeCoin(int count)
    {
        if (count > 0)
        {
            wallet.changeCount.color = new Color32(50, 255, 0, 255);
            wallet.changeCount.text = "+" + count;
        }
        else if (count < 0)
        {
            wallet.changeCount.color = new Color32(234, 255, 0, 255);
            wallet.changeCount.text = "" + count;
        }
        wallet.coinUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(726, -483), 1f);
        DOVirtual.DelayedCall(3f, () =>
        {
            wallet.hideWallet();
        });
        DataManager.Instance.data.coinCount += count;
        wallet.coinCount.text = DataManager.Instance.data.coinCount.ToString();
        DataManager.Instance.savePlayerData();
    }
}
