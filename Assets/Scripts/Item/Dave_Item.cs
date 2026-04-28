using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class Dave_Item : Item
{
    public string itemTag;
    private void Start()
    {
        float flyRange = Random.Range(1f, 2f);
        float flyHeight = Random.Range(1f, 2f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x - flyRange, transform.position.y + flyHeight, 0), new Vector3(transform.position.x - flyRange * 2, transform.position.y - flyHeight, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
    public void addDave()
    {
        DataManager.Instance.data.hasDave = true;
        if (itemTag == "ITEM2")
        {
            DataManager.Instance.data.timeC1L8++;
        }
    }
    public override void clickEvent()
    {
        addDave();
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
