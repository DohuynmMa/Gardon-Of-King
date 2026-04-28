using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Coin : Item
{
    public bool hasClicked;
    public string itemTag;
    private int addCoinCount = 1;
    private void Start()
    {
        hasClicked = false;
        DOVirtual.DelayedCall(2f / DataManager.Instance.data.gameSpeed, () =>
        {
            if (!hasClicked)
            {
                hasClicked = true;
                destroyItem();
                Sounds.coin_click.play();
            }
        });
        float moveX = Random.Range(-0.5f, 0.5f);
        float moveY = Random.Range(0.5f, 0.8f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x + moveX, transform.position.y + moveY, 0) ,new Vector3(transform.position.x + moveX, transform.position.y - moveY, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    private void Update()
    {
        Quaternion rot = transform.rotation;//½ð±ÒÐý×ª
        if (rot.y > 360) rot.y = 0;
        else rot.y += Time.deltaTime * 2f;
        transform.rotation = rot;
    }
    public void addCoin()
    {
        CoinManager.Instance.changeCoin(addCoinCount);
    }
    public override void clickEvent()
    {
        addCoin();
        if (!hasClicked)
        {
            hasClicked = true;
            Sounds.coin_click.play();
        }
        base.clickEvent();
    }
    public override void destroyItem()
    {
        transform.DOPath(new Vector3[] { new Vector3(5.2f,-3.9f,0) }, 2f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(()=> {
            Destroy(gameObject);
        });
    }
}
