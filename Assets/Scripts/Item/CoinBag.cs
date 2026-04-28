using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;

public class CoinBag : Item
{
    public string itemTag;
    public int addCoinCount;
    public bool gameWin;
    void Start()
    {
        if (itemTag == "COIN2") DOVirtual.DelayedCall(20f / DataManager.Instance.data.gameSpeed, () =>
        {
            Destroy(gameObject);
        });
        var flyRange = Random.Range(1f, 2f);
        var flyHeight = Random.Range(1f, 2f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x - flyRange, transform.position.y + flyHeight, 0), new Vector3(transform.position.x - flyRange * 2, transform.position.y - flyHeight, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    public void addCoin()
    {
        CoinManager.Instance.changeCoin(addCoinCount);
        if (gameWin)
        {
            Musics. §¿˚“Ù¿÷.play(false);
            GameManager.Instance.changeScene("GameMenu", 5, GameMode.None, true);
        }
    }
    public override void clickEvent()
    {
        addCoin();
        base.clickEvent();
    }
    public override void destroyItem()
    {
        var breakEff = Instantiate(Utils.findEffectByType(AreaEffectType.CoinSpread), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
