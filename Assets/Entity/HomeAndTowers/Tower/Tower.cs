using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
public class Tower : MonoBehaviour
{
    public int towerID;
    public float towerHp;
    public float towerMaxHp;
    public float attackTime;
    public float attackRange;
    public float sigahtRange;
    public float costPoint;
    private float costPointTimer;
    public Entity attachEntity;
    public GameObject trueHpBar;
    public Image trueHpBarInner;
    public Image costPointIcon;
    public GameObject trueHpBarIcon;

    public float offsetX;
    public float offsetY;
    public EntityGroup entityGroup;
    public Sprite friendSkin;
    public Sprite enemySkin;
    Vector3 offsetPos;
    private void Update()
    {
        if (MultiGameManager.client == null) updateHpBar();
        costPointUpdate();
        if (!DataManager.Instance.data.showHpBar || (!attachEntity.onSpriteMask && Utils.inNight()) || !GameManager.Instance.inGame)
        {
            trueHpBar.gameObject.SetActive(false);
            return;
        }
        trueHpBar.gameObject.SetActive(true);
        Utils.uiMove(trueHpBar, offsetPos);
    }
    public void updateHpBar()
    {
        trueHpBarInner.fillAmount = towerHp / towerMaxHp;
    }
    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm.gameMode == GameMode.Normal || gm.gameMode == GameMode.MultiPlayer || gm.gameMode.isMiniGame())
        {
            if (entityGroup == EntityGroup.enemy)
            {
                GetComponent<SpriteRenderer>().sprite = enemySkin;
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = friendSkin;
            }
        }

        offsetPos = transform.position;

        offsetPos.x += (entityGroup == EntityGroup.friend ? -0.6f : 0.6f);
        offsetPos.y += (tag == "Home" ? 4f : 0.6f);

        trueHpBar.GetComponent<RectTransform>().localScale *= 0.5f;

        var image = trueHpBarIcon.GetComponent<Image>();
        var hpIcons = trueHpBarIcon.GetComponent<Entity_Hp_Icon>();
        if (entityGroup == EntityGroup.friend) image.sprite = ImageManager.Instance.hpBarIcon[2];
        else image.sprite = ImageManager.Instance.hpBarIcon[3];
    }
    public void towerBreak()
    {
        var gm = GameManager.Instance;
        if (attachEntity.anim != null)
        {
            attachEntity.anim.SetBool("Die", true);
        }
        //爆炸特效
        Instantiate(Utils.findEffectByType(entityGroup == EntityGroup.friend ? AreaEffectType.TowerBreakBlue : AreaEffectType.TowerBreakRed),transform.position,Quaternion.identity);

        if (entityGroup == EntityGroup.friend)
        {
            if (gm.home1.homeState == HomeState.Disactivate) gm.home1.transitionToActivate();
            gm.friendTowerCount--;
        }
        else if (entityGroup == EntityGroup.enemy)
        {
            if (gm.home2.homeState == HomeState.Disactivate) gm.home2.transitionToActivate();
            gm.enemyTowerCount--;
        }
        switch (towerID)
        {
            case 1:
                gm.tower1 = null;
                break;
            case 2:
                gm.tower2 = null;
                break;
            case 3:
                gm.tower3 = null;
                break;
            case 4:
                gm.tower4 = null;
                break;
            default:
                break;
        }
        gm.towerUpdate();
        Destroy(gameObject);
    }
    public void updateTranslate()
    {
        int nowLevel = DataManager.Instance.data.towerLevel;

        GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.towerSkins[nowLevel - 1];
        switch (nowLevel)
        {
            case 1:
                towerMaxHp = 90;
                break;
            case 2:
                towerMaxHp = 105;
                break;
            case 3:
                towerMaxHp = 120;
                break;
            case 4:
                towerMaxHp = 135;
                break;
            case 5:
                towerMaxHp = 150;
                break;
            case 6:
                towerMaxHp = 200;
                break;
        }
    }
    public void putEntity(Vector3 currentPos,EntityType type)
    {
        var currentEntity = Instantiate(HandManager.Instance.getEntityPrefeb(type), currentPos, Quaternion.identity);
        setCostPoint(type);
        currentEntity.maxHitpoint = towerMaxHp;
        currentEntity.hitpoint = towerHp;

        currentEntity.entityGroup = entityGroup;
        currentEntity.transform.parent = transform;
        currentEntity.moveSpeed = 0;
        currentEntity.lifeTime = 0;

        currentEntity.anim.enabled = true;

        currentEntity.entityState = EntityState.enable;
        currentEntity.deployShadow.SetActive(false);
        currentEntity.entityShadow.SetActive(false);
        Vector2 colliderPos2 = currentEntity.boxCollider.bounds.center;
        float yPos = colliderPos2.y;
        foreach (var spriteRenderer in currentEntity.gameObject.getAllSR())
        {
            if (spriteRenderer.gameObject.tag == "Shadow") continue;
            int baseSortOrder = spriteRenderer.sortingOrder;
            int dynamicOrder = baseSortOrder - Mathf.RoundToInt(yPos * 1000);
            spriteRenderer.sortingOrder = dynamicOrder;
            GetComponent<SpriteRenderer>().sortingOrder = dynamicOrder - 1;
        }
        attachEntity = currentEntity;

        if (GameManager.Instance.gameMode == GameMode.MultiPlayer) setTowerEntityId();
    }
    private void setCostPoint(EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.PeaShooter:
                costPoint = 0;
                break;
            case EntityType.SunFlower:
                costPoint = 1;
                break;
            case EntityType.Cabbage:
                costPoint = 0;
                break;
            case EntityType.WallNut:
                costPoint = 0;
                break;
            case EntityType.SnowPeaShooter:
                costPoint = 1;
                break;
            case EntityType.GatlingPeaShooter:
                costPoint = 2;
                break;
            case EntityType.Cornpult:
                costPoint = 0;
                break;
            case EntityType.Watermelon:
                costPoint = 2;
                break;
            case EntityType.Npeashooter:
                costPoint = 1;
                break;
            case EntityType.IceMelon:
                costPoint = 3;
                break;
            case EntityType.LittleWolf:
                costPoint = 2;
                break;
        }
        costPointIcon.gameObject.SetActive(costPoint != 0);
    }
    private void costPointUpdate()
    {
        if (costPoint == 0) return;
        costPointTimer += Time.deltaTime;
        costPointIcon.fillAmount = costPointTimer / 30f;
        if (costPointTimer >= 30 && SunManager.Instance.sunPoint >= costPoint)
        {
            SunManager.Instance.changeSun(costPoint * -1f / (GameManager.Instance.friendTowerCount != 0 ? GameManager.Instance.friendTowerCount : 2f));
            costPointTimer = 0;
            //todo 扣费特效
        }
    }
    private void setTowerEntityId()
    {
        if(MultiGameManager.server != null)
        {
            attachEntity.entityID = towerID;
        }
        if(MultiGameManager.client != null)
        {
            switch (towerID)
            {
                case 1:
                    attachEntity.entityID = 3;
                    break;
                case 2:
                    attachEntity.entityID = 4;
                    break;
                case 3:
                    attachEntity.entityID = 1;
                    break;
                case 4:
                    attachEntity.entityID = 2;
                    break;
            }
        }
    }
}
