using Assets.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
public class AreaEffectEntity : Entity
{
    [Header("AreaEffectEntity:Entity")]
    public AreaEffectType spawningEffectType = AreaEffectType.None;
    public Sounds spawnEffectSounds = Sounds.ÎÞÒôÐ§;
    public Vector3 spawnPos;
    public override void Start()
    {
        changeMaterial();
        maxHitpoint = 99999;
        hitpoint = 99999;
    }
    public override void Update()
    {
    }
    private void spawnEffectByAnim()
    {
        var gm = GameManager.Instance;
        if (spawningEffectType == AreaEffectType.None)
        {
            print("no effect");
            return;
        }
        var effect = Instantiate(Utils.findEffectByType(spawningEffectType),transform.position,Quaternion.identity);
        effect.summonner = entityGroup == EntityGroup.friend ? gm.home1 : gm.home2;
        spawnEffectSounds.play();
        transform.DOPath(new Vector3[] { new Vector3(Random.Range(-14f, 14f), 15, 0)}, 3f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            entityDie();
        });
    }
    public override void transitionToDisable()
    {
        agent.enabled = false;
        anim.enabled = false;
        entityState = EntityState.disable;
    }
    public override void transitionToEnable() {
        agent.enabled = true;
        anim.enabled = true;
        entityState = EntityState.enable;
        GetComponent<SpriteRenderer>().sprite = null;
        transform.DOPath(new Vector3[] { spawnPos }, 1.5f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        deployShadow.GetComponent<SpriteRenderer>().sprite = null;
        changeSkin();
    }
    public override void updateHpBarImage()
    {

    }
    public override void entityDie()
    {
        Destroy(gameObject);
    }
    public override void addToCellEvent(Cell cell)
    {
        transform.position = new Vector3(Random.Range(-14f, 14f), 15, 0);
        spawnPos = cell.entityPosition(this);
    }
}
