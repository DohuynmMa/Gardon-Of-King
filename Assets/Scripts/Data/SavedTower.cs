using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SavedTowerHelper
{
    public static SavedTower save(this Tower tower)
    {
        var entity = tower.attachEntity.save();
        return new()
        {
            instanceId = tower.GetInstanceID(),
            towerID = tower.towerID,
            towerHp = tower.towerHp,
            towerMaxHp = tower.towerMaxHp,
            attackTime = tower.attackTime,
            attackRange = tower.attackRange,
            sigahtRange = tower.sigahtRange,
            attachEntity = entity,
            transformX = tower.transform.position.x,
            transformY = tower.transform.position.y,
            offsetX = tower.offsetX,
            offsetY = tower.offsetY,
            entityGroup = tower.entityGroup,
        };
    }
}

[System.Serializable]
public class SavedTower
{
    public int instanceId;
    public int towerID;
    public float towerHp;
    public float towerMaxHp;
    public float attackTime;
    public float attackRange;
    public float sigahtRange;

    public SavedEntity attachEntity;

    public float transformX; // 坐标
    public float transformY; // 坐标

    public float offsetX;
    public float offsetY;

    public EntityGroup entityGroup;

    public void load()
    {
        Vector3 pos = new Vector3(transformX, transformY, 0);
        var tower = GameObject.Instantiate(GameManager.Instance.tower, pos, Quaternion.identity).GetComponent<Tower>();

        Vector3 currentPos = tower.transform.position;
        currentPos.x += tower.GetComponent<Tower>().offsetX; currentPos.y += tower.GetComponent<Tower>().offsetY;

        tower.GetComponent<Tower>().updateTranslate();
        tower.GetComponent<Tower>().putEntity(currentPos, DataManager.Instance.data.towerEntity);

        apply(tower.GetComponent<Tower>());

        tower.GetComponent<Tower>().attachEntity.maxHitpoint = tower.GetComponent<Tower>().towerMaxHp;
        tower.GetComponent<Tower>().attachEntity.hitpoint = tower.GetComponent<Tower>().towerHp;

        if (towerID == 1) GameManager.Instance.tower1 = tower;
        if (towerID == 2) GameManager.Instance.tower2 = tower;
    }

    public void apply(Tower tower)
    {
        tower.towerID = towerID;
        tower.towerHp = towerHp;
        tower.towerMaxHp = towerMaxHp;
        tower.attackTime = attackTime;
        tower.attackRange = attackRange;
        tower.sigahtRange = sigahtRange;
        tower.offsetX = offsetX;
        tower.offsetY = offsetY;
        tower.entityGroup = entityGroup;
        tower.trueHpBarInner.sprite = ImageManager.Instance.hpBarImages[entityGroup == EntityGroup.friend ? 4 : 5];
        //tower.markToLoad(attachEntity);
    }
    public override bool Equals(object obj)
    {
        return obj is SavedTower ? ((SavedTower)obj).instanceId == instanceId : false;
    }

    public override int GetHashCode()
    {
        return instanceId;
    }
}
