using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
public class BossDaveAWM : MonoBehaviour
{
    public float volumeTime;

    public int id;

    public BossDave bossDave;

    public Entity currentAim;//当前目标

    public Home_Target target;//准心

    public float shootDuration = 8;
    private float shootTimer;

    public void Update()
    {
        if (bossDave.daveBossState == BossDave.DaveBossState.shoot)
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            target.appear();
            shootTimer += Time.deltaTime;
            aimingToAimToShoot();
        }
        else
        {
            target.hide();
            GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        }
    }

    public void aimingToAimToShoot()
    {
        if (currentAim == null)
        {
            currentAim = findingAim();
            target.hide();
            return;
        }
        if (currentAim.hitpoint > 0)
        {
            target.appear();
            target.moveTo(currentAim);
        }
        if (shootTimer >= shootDuration)
        {
            if (target.isAimed)
            {
                shootTimer = 0;
                GetComponent<Animator>().SetTrigger("attack");
            }
        }
    }
    private Entity findingAim()
    {
        float shootRange = 9;
        float closestDistance = 999;
        GameObject closestAim = null;

        var allEntities = GameObject.FindGameObjectsWithTag("Entity");

        foreach (var entityObject in allEntities)
        {
            if (entityObject == null || !entityObject.activeSelf)
                continue;

            var entity = entityObject.GetComponent<Entity>();

            if (entity == null || entity.hitpoint <= 0)
                continue;

            if (entityObject == gameObject || entity.entityState == EntityState.disable || entity.entityGroup == EntityGroup.enemy || entity.isAreaEffect())
                continue;

            if (entity.tag == "Home" || entity.hasParent)
                continue;

            if (entity.GetComponent<MinerZombie>() != null)
            {
                if (entity.GetComponent<MinerZombie>().inMining) continue;
            }

            float distance = Vector3.Distance(entity.getEntityBoxColliderPos(), transform.position);

            //坚果会成为优先攻击目标
            if (entity.entityType == EntityType.WallNut && distance <= shootRange) return entity;

            if (distance > shootRange)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAim = entityObject;
            }
        }

        if (closestAim != null)
            return closestAim.GetComponent<Entity>();
        else return null;
    }
    private void shoot()
    {
        if (currentAim == null)
        {
            return;
        }
        CameraManager.Instance.shake(-0.2f);
        var aimPos = currentAim.getEntityBoxColliderPos();
        Instantiate(Utils.findEffectByType(AreaEffectType.ZombieBlood), aimPos, Quaternion.identity);
        currentAim.changeHitpoint(10,null,bossDave);
        currentAim = null;
        target.isAimed = false;
    }
    private void playSound(int ID)//动画
    {
        SoundsManager.playSounds(ID, volumeTime);
    }
    private void playVoice(int ID)//动画
    {
        SoundsManager.playVoice(ID);
    }
}
