using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public static class SavedEntityHelper
{
    public static SavedEntity save(this Entity entity)
    {
        if (entity.entityState == EntityState.disable) return null;
        return new()
        {
            instanceId = entity.GetInstanceID(),
            tag = entity.gameObject.tag,
            entityGroup = entity.entityGroup,
            entityType = entity.entityType,
            transformX = entity.transform.position.x,
            transformY = entity.transform.position.y,
            hitpoint = entity.hitpoint,
            maxHitpoint = entity.maxHitpoint,
            lifeTime = entity.lifeTime,
            lifeTimer = entity.lifeTimer,
            canGains = entity.canGains,
            golden = entity.golden,
            offsetX = entity.offsetX,
            offsetY = entity.offsetY,
            bulletOffsetX = entity.bulletOffsetX,
            bulletOffsetY = entity.bulletOffsetY,
            moveSpeed = entity.moveSpeed,
            hitTime = entity.hitTime,
            damage = entity.damage,
            range = entity.range,
            sightRange = entity.sightRange,

            maxArmorHp = entity.GetComponent<ArmorZombie>() != null ? entity.GetComponent<ArmorZombie>().maxArmorHp : 0,
            armorHp = entity.GetComponent<ArmorZombie>() != null ? entity.GetComponent<ArmorZombie>().armorHp : 0,
            hasArmor = entity.GetComponent<ArmorZombie>() != null ? entity.GetComponent<ArmorZombie>().hasArmor : false,

            hasThrowSon = entity.GetComponent<GiantZombie>() != null ? entity.GetComponent<GiantZombie>().hasThrowSon : false,
            hasSon = entity.GetComponent<GiantZombie>() != null ? entity.GetComponent<GiantZombie>().hasSon : false,

            hasShield = entity.GetComponent<ShieldZombie>() != null ? entity.GetComponent<ShieldZombie>().hasShield : false,
            shieldMaxHitpoint = entity.GetComponent<ShieldZombie>() != null ? entity.GetComponent<ShieldZombie>().shieldMaxHitpoint : 0,
            shieldHitpoint = entity.GetComponent<ShieldZombie>() != null ? entity.GetComponent<ShieldZombie>().shieldHitpoint : 0,

        };
    }
}

[System.Serializable]
public class SavedEntity
{
    public int instanceId;
    public string tag;
    public EntityGroup entityGroup;//阵营
    public EntityType entityType;//实体类型

    public float transformX; // 坐标
    public float transformY; // 坐标

    public float hitpoint;//当前血量
    public float maxHitpoint;//最高血量

    public float lifeTime;//存活时间
    public float lifeTimer;//尸潮如水限定
    public bool canGains;//尸潮如水限定,是否可收获(植物)
    public bool golden;//尸潮如水限定,是否僵尸为GOLDEN(增强属性和掉落奖励)

    public float offsetY;//偏移X坐标
    public float offsetX;//偏移Y坐标
    public float bulletOffsetY;//子弹偏移X坐标
    public float bulletOffsetX;//子弹偏移Y坐标

    public float moveSpeed;//移速

    public float hitTime;//攻击间隔

    public float damage;//若近战,近战的伤害(子弹为null)

    public float range;//攻击范围
    public float sightRange;//视野范围,用于找目标

    //防具僵尸限定
    public float maxArmorHp;
    public float armorHp;
    public bool hasArmor;
    //类似巨人僵尸限定
    public bool hasSon;
    public bool hasThrowSon;
    //有盾牌僵尸限定
    public bool hasShield;
    public float shieldMaxHitpoint;
    public float shieldHitpoint;
    private bool loadHome(Vector3 position)
    {
        if (tag != "Home") return false;
        var gm = GameManager.Instance;
        var isFriend = entityGroup == EntityGroup.friend;

        if (isFriend)
        {
            // 限制只能存在一个家
            if (gm.home1 != null && gm.home1.gameObject.activeInHierarchy)
            {
                MonoBehaviour.print("己方已存在家，不读取实体数据");
                return true;
            }
        }
        else
        {
            // 限制只能存在一个家
            if (gm.home2 != null && gm.home2.gameObject.activeInHierarchy) return true;
        }

        var home = GameObject.Instantiate(GameManager.Instance.home, position, Quaternion.identity).GetComponent<Home>();
        home.entityGroup = entityGroup;
        home.transitionToDisable();
        home.hpBar.sprite = ImageManager.Instance.hpBarImages[isFriend ? 8 : 9];
        home.hpBarInner.sprite = ImageManager.Instance.hpBarImages[isFriend ? 6 : 7];
        home.hpBar.GetComponent<RectTransform>().localScale *= 1.5f;
        home.updateHomeTranslate();
        apply(home);
        home.updateHpBar();
        if (isFriend) gm.home1 = home;
        else gm.home2 = home;

        //生成守卫者
        if (DataManager.Instance.data.homeNpc != HomeNpcType.none && entityGroup == EntityGroup.friend)
        {
            HomeNpc homeNpc = GameObject.Instantiate(GameManager.Instance.getHomeNpcPrefab(DataManager.Instance.data.homeNpc));
            homeNpc.home = home;

            homeNpc.transform.position = new Vector3(entityGroup == EntityGroup.friend ? -8.46f : 8.538f, -3.83f, 0);
            homeNpc.transform.localScale = new Vector3(entityGroup == EntityGroup.friend ? -0.45f : 0.45f, 0.45f, 0.45f);
            homeNpc.target.GetComponent<SpriteRenderer>().sprite = homeNpc.target.GetComponent<Home_Target>().aimSprite[entityGroup == EntityGroup.enemy ? 1 : 0];

            home.npc = homeNpc;
            home.npc.gameObject.SetActive(false);
        }

        return true;
    }

    public void load()
    {
        var position = new Vector3(transformX, transformY, 0);
        if (loadHome(position)) return;
        Entity entityPrefeb = HandManager.Instance.getEntityPrefeb(entityType);
        if (entityPrefeb == null)
        {
            //MonoBehaviour.print("实体预制体加载失败，无法找到类型为" + entityType.DisplayName() + "的预制体");
            return;
        }
        var currentEntity = GameObject.Instantiate(entityPrefeb, position, Quaternion.identity);
        apply(currentEntity);
        currentEntity.updateHpBar();
        currentEntity.updateHpBarImage();
        //currentEntity.GetComponent<Animator>().speed = 1f;
        if (currentEntity.hitpoint <= 0) currentEntity.entityDie();
        //这里会特殊处理位置和大小偏移
        if (currentEntity.GetComponent<SunFlower>() != null || currentEntity.GetComponent<WallNut>() != null)
        {
            currentEntity.transform.localScale = new Vector3(1, 1, 1);
        }
        currentEntity.transitionToEnable();
        currentEntity.entityGroup = entityGroup;
        if (currentEntity.moveSpeed != 0) currentEntity = null;
        else currentEntity.lockPos = position;
    }
    public void apply(Entity entity)
    {
        entity.oldInstanceID = instanceId;
        entity.entityGroup = entityGroup;
        entity.entityType = entityType;
        entity.entityState = EntityState.enable;
        entity.hitpoint = hitpoint;
        entity.maxHitpoint = maxHitpoint;
        entity.lifeTime = lifeTime;
        entity.lifeTimer = lifeTimer;
        entity.canGains = canGains;
        entity.golden = golden;
        entity.offsetX = offsetX;
        entity.offsetY = offsetY;
        entity.bulletOffsetX = bulletOffsetX;
        entity.bulletOffsetY = bulletOffsetY;
        entity.moveSpeed = moveSpeed;
        entity.hitTime = hitTime;
        entity.damage = damage;
        entity.range = range;
        entity.sightRange = sightRange;
        if(entity.GetComponent<ArmorZombie>() != null)
        {
            entity.GetComponent<ArmorZombie>().maxArmorHp = maxArmorHp;
            entity.GetComponent<ArmorZombie>().armorHp = armorHp;
            entity.GetComponent<ArmorZombie>().hasArmor = hasArmor;
        }
        else if(entity.GetComponent<GiantZombie>() != null)
        {
            entity.GetComponent<GiantZombie>().hasThrowSon = hasThrowSon;
            entity.GetComponent<GiantZombie>().hasSon = hasSon;
        }
        else if (entity.GetComponent<ShieldZombie>() != null)
        {
            entity.GetComponent<ShieldZombie>().shieldMaxHitpoint = shieldMaxHitpoint;
            entity.GetComponent<ShieldZombie>().shieldHitpoint = shieldHitpoint;
        }
    }
    public override bool Equals(object obj)
    {
        return obj is SavedEntity ? ((SavedEntity)obj).instanceId == instanceId : false;
    }

    public override int GetHashCode()
    {
        return instanceId;
    }
}
