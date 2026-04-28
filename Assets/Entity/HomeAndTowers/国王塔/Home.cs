using UnityEngine;
using Assets.Scripts.Utils;
public enum HomeState
{
    Activate,
    Disactivate
}
public class Home : Entity
{
    [Header("Home")]
    public HomeState homeState = HomeState.Disactivate;
    public HomeNpc npc;
    private bool die = false;
    public override void Start()
    {
        base.Start();
        setHomeEntityId();
    }
    public override void Update()
    {
        base.Update();
        switch (homeState)
        {
            case HomeState.Activate:
                activateUpdate();
                break;
            case HomeState.Disactivate:
                disactivateUpdate();
                break;
        }
    }
    public void updateHomeTranslate()
    {
        if(entityGroup == EntityGroup.enemy)
        {
            hpBar.gameObject.SetActive(false);
            maxHitpoint = 9999999;
            hitpoint = maxHitpoint;
        }
        else
        {
            var nowLevel = DataManager.Instance.data.towerLevel;
            switch (nowLevel)
            {
                case 1:
                    maxHitpoint = 150;
                    break;
                case 2:
                    maxHitpoint = 180;
                    break;
                case 3:
                    maxHitpoint = 210;
                    break;
                case 4:
                    maxHitpoint = 240;
                    break;
                case 5:
                    maxHitpoint = 300;
                    break;
                case 6:
                    maxHitpoint = 400;
                    break;
            }
        }
    }
    private void activateUpdate()
    {
        if (npc == null) return;
        npc.npcUpdate();
    }
    private void disactivateUpdate()
    {
        if (hitpoint != maxHitpoint) transitionToActivate();
    }
    public void transitionToActivate()
    {
        homeState = HomeState.Activate;
        if (npc == null) return;
        npc.gameObject.SetActive(true);
        npc.GetComponent<Animator>().SetTrigger("激活");
    }
    private void setHomeEntityId()
    {
        if (GameManager.Instance.gameMode != GameMode.MultiPlayer) return;
        die = false;
        if (MultiGameManager.server != null)
        {
            entityID = entityGroup == EntityGroup.friend ? 5 : 6;
        }
        if (MultiGameManager.client != null)
        {
            entityID = entityGroup == EntityGroup.friend ? 6 : 5;
        }
    }
    public override void transitionToDisable()
    {
        entityState = EntityState.disable;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.gameObject.tag == "Shadow") continue;
            spriteRenderer.sortingLayerName = GetComponent<MinerZombie>() == null ? "Other" : "Entity";
        }
        transitionToEnable();
    }
    public override void transitionToEnable()
    {
        entityState = EntityState.enable;
    }
    public override void enableUpdate()
    {
        hpBarUpdate();
    }
    public override bool changeHitpoint(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        if (GameManager.Instance.gameMode == GameMode.SRCS)
        {
            if (entityGroup == EntityGroup.enemy) DataManager.Instance.data.scoreSRCS += damage * 0.5f * (1 + DataManager.Instance.data.towerLevel * 0.1f);
            DialogKingShopFix.Instance.updateFixUI();
        }
        //BS敌方的家门无法被攻击
        if (GameManager.Instance.gameMode == GameMode.MiniGame_BS && entityGroup == EntityGroup.enemy)
        {
            return base.changeHitpoint(0, damageBullet, damager, deltaTimeDamage);
        }
        return base.changeHitpoint(damage,damageBullet, damager, deltaTimeDamage);
    }
    public override void entityDie()
    {
        if (die) return;
        die = true;
        if (entityGroup == EntityGroup.friend)
        {
            GameManager.Instance.gameOver(EntityGroup.enemy);
        }
        if (entityGroup == EntityGroup.enemy && GameManager.Instance.level != 8)
        {
            print("赢了!!!");
            GameManager.Instance.gameOver(EntityGroup.friend);
        }
        base.entityDie();
    }
    public override void modeSet()
    {
    }
    public override void updateHpBarImage()
    {
    }
    public override void hpBarUpdate()
    {
        if(TestManager.Instance.videoMode && GameManager.Instance.chapter == 99 && GameManager.Instance.level == 1)//录视频模式不显示血条
        {
            hpBar.gameObject.SetActive(false);
            return;
        }
        base.hpBarUpdate();
    }
}
