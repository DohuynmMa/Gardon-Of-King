using Assets.Scripts.Utils;
using UnityEngine;

public class WitchZombie : Zombie
{
    public override void transitionToEnable()
    {
        Sounds.女巫生成.playWithPitch();
        base.transitionToEnable();
    }
    public override void summonEntityUpdate()
    {
        if (!summonEntity) return;
        summonTimer += Time.deltaTime;
        if (summonTimer >= summonDuartion / (golden ? 2 : 1))
        {
            summon_Entity();
        }
    }
    public override void summon_Entity()
    {
        moveSpeed = 0.000001f;
        agent.speed = moveSpeed;
        anim.SetTrigger("summon entity");
    }
    private void afterSummonByAnim()
    {
        moveSpeed = 0.35f;
        agent.speed = moveSpeed;
    }
    private void summonByAnim()
    {
        Vector3 pos = this.getEntityBoxColliderPos();
        Sounds.女巫召唤.playWithPitch();
        pos.y -= 0.4f;
        for (int i = 0; i < summonCount; i++)
        {
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
    private void resetSpawnTimer()
    {
        summonTimer = 0;
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("帽子").gameObject.SetActive(false);
        transform.GetChild(0).Find("下巴").gameObject.SetActive(false);
        transform.GetChild(0).Find("头").gameObject.SetActive(false);
        base.hideHead();
    }
}
