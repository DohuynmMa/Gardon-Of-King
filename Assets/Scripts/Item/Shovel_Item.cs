using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class Shovel_Item : Item
{
    public string itemTag;
    private void Start()
    {
        float flyRange = Random.Range(1f, 2f);
        float flyHeight = Random.Range(1f, 2f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x - flyRange, transform.position.y + flyHeight, 0), new Vector3(transform.position.x - flyRange * 2, transform.position.y - flyHeight, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    public void addShovel()
    {
        DataManager.Instance.data.hasShovel = true;
        if (itemTag == "ITEM1")
        {
            DataManager.Instance.data.timeC1L4++;
        }
    }
    public override void clickEvent()
    {
        addShovel();
        Sounds.prize_click.play();
        base.clickEvent();
    }
    public override void destroyItem()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.SpreadingStar), transform.position, Quaternion.identity);
        DataManager.Instance.savePlayerData();
        Destroy(gameObject);
    }
}
