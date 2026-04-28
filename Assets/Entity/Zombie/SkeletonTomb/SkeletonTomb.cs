using Assets.Scripts.Utils;
using UnityEngine;

public class SkeletonTomb : Zombie
{
    [Header("SkeletonTomb:Zombie")]
    public int summonSkeletonCountWhenDie = 1;
    private void summonSkeletonsWhenDieByAnim()
    {
        for (int i = 0; i < summonSkeletonCountWhenDie; i++) {
            cell.addEntityDirectly(Instantiate(HandManager.Instance.getEntityPrefeb(EntityType.Skeleton)), entityGroup);
        }
    }
    public override void summon_Entity()
    {
        Vector3 pos = this.getEntityBoxColliderPos();
        pos.y -= 0.4f;
        var entity = Instantiate(HandManager.Instance.getEntityPrefeb(summonEntityType), pos, Quaternion.identity);
        var skeleton = entity.GetComponent<Skeleton>();
        skeleton.transitionToDisable();
        if (skeleton != null)
        {
            skeleton.anim.SetBool("inDirt", true);
        }
        entity.entityGroup = entityGroup;
        entity.lifeTimer = 0;
        entity.hitpoint = entity.maxHitpoint;
        entity.updateHpBarImage();
        entity.entityState = EntityState.enable;
        entity.anim.enabled = true;
        entity.entityShadow.SetActive(true);
        entity.deployShadow.SetActive(false);
        entity.agent.enabled = true;
        entity.addToCellEvent(cell);
    }
}
