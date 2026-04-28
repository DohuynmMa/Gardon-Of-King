using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork.Packet.Play.Client;
public enum EntityState
{
    disable,
    enable
}
public enum EntityGroup
{
    friend,
    enemy,
}
public class Entity : MonoBehaviour
{
    #region 数据
    [HideInInspector] public int oldInstanceID;
    [HideInInspector] public int[] baseSortingOrders;//初始层级
    private const int orderOffset = 1000;//层级偏移
    protected internal Animator anim;
    protected internal BoxCollider2D boxCollider;
    protected internal NavMeshAgent agent;//AI组件
    protected internal SpriteRenderer spriteRenderer;//渲染器
    protected internal SpriteRenderer[] spriteRenderers;//所有子类的渲染器
    
    public EntityGroup entityGroup;//阵营
    public EntityType entityType;//实体类型
    public EntityState entityState;//实体状态

    /************** 预制体固定资源 - 开始 *************/
    [Header("必要组件和设置")]
    public GameObject deployShadow;//放置的透明影子
    public GameObject entityShadow;//下方的影子

    public Image hpBar;//血条外壳
    public Image hpBarInner;//血条内部
    public Image gainBar;//收获条(尸如潮水)
    public Image gainBarInner;//收获条内部(尸如潮水)
    public Image gainBarIcon;//收获条图标

    public Sounds attackSounds = Sounds.无音效; //攻击音效

    public Bullet bullet;//发射的子弹,无子弹则默认为近战

    [HideInInspector] public bool hasParent;//是否有父对象,用于判断是否为皇家塔或者巨人背后的小鬼

    [HideInInspector] public GameObject parentEntity;//是否有父对象的实体

    [HideInInspector] public Vector3 lockPos;//移速为0的物体的锁定坐标,放置被别人挤走

    [Header("可选设置")]
    public List<GameObject> changeGroupSkin; //会根据队伍改变皮肤的位置

    public List<Sprite> blueSkin; //蓝队对应皮肤图片
    public List<Sprite> redSkin; //红队对应皮肤图片

    [HideInInspector] public List<Buff> buffList;
    public bool fixAnim = false;//是否保持动画默认第一帧?
    public bool fixDeployShadowPos = false;//是否修整DEPLOY位置
    /************** 预制体固定资源 - 结束 *************/

    public Cell cell;//脚下格子

    public Entity aim;//目标
    public List<Entity> ignoreAims;
    public List<Entity> randomAims;
    [HideInInspector] public Entity aimNoSure;//第二目标
    [Header("属性")]
    public float hitpoint;//当前血量
    public float maxHitpoint;//最高血量

    public float lifeTime;//存活时间
    [HideInInspector]public float lifeTimer;

    //尸潮如水限定
    [HideInInspector] public bool canGains;//是否可收获(植物)
    [HideInInspector] public bool golden;//是否僵尸为GOLDEN(增强属性和掉落奖励)
    [HideInInspector] public bool canWatering;//是否可浇水
    public bool needingWater;//是否需要水
    [HideInInspector] public float needWaterTimer;
    [HideInInspector] private float needWaterDuration;//距离下次需要浇水

    public float moveSpeed;//移速

    public float hitTime;//攻击间隔
    [HideInInspector]public float hitTimer;
    public bool noTimerAttack = false;
    private float aimTimer; // 更换目标计时器，暂定为反应时间与攻击间隔相同

    public float damage;//若近战,近战的伤害(子弹为null)

    public float range;//攻击范围
    public float sightRange;//视野范围,用于找目标

    public List<BuffType> avoidingBuff;//免疫的buff

    public float reflectDamage;//被攻击后反伤大小
    public BuffType reflectBuffType = BuffType.none;//被攻击后给攻击者的buff TYPE
    public float reflectBuffTime;//被攻击后给攻击者的buff时间

    public float startFindAimTimeAfterDeploy;//开始寻找目标间隔时间
    private float startFindAimTimerAfterDeploy;

    public bool isLockPos = false;
    public bool canBePushback = true;
    public bool summonEntity = false;
    protected float summonTimer;
    public float summonDuartion;
    public int summonCount = 1;
    public EntityType summonEntityType;

    public bool onSpriteMask = false;


    [Header("偏移")]
    public float offsetY;//偏移X坐标
    public float offsetX;//偏移Y坐标
    public float bulletOffsetY;//子弹偏移X坐标
    public float bulletOffsetX;//子弹偏移Y坐标
    public float hpBaroffsetX;//血条偏移Y坐标
    public float hpBaroffsetY;//血条偏移Y坐标

    /// <summary>
    /// Entity ID MultiGame
    /// </summary>
    [Header("联机")]
    public int entityID;//多人联机的实体ID
    private float syncTimer = 0.4f;//每隔一段时间同步

    [Header("MiniGames")]
    private float bombTimer;
    #endregion
    public virtual void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        agent = GetComponent<NavMeshAgent>();
    }
    public virtual void Start()
    {
        //实体初始化

        //防御塔特殊设置
        if (transform.parent != null)
        {
            parentEntity = transform.parent.gameObject;
            hasParent = true;
            transform.rotation = new Quaternion(0, 0, 0,transform.rotation.w);
            agent.height += 0.5f;
            agent.radius = 0.8f;
            hpBar.rectTransform.localScale *= 1.5f;
            agent.enabled = true;
            anim.enabled = true;
            lockPos = transform.position;
            return;
        }
        agent.stoppingDistance = range;
        agent.speed = moveSpeed;
        hasParent = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        spriteRenderers = gameObject.getAllSR();
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            return;
        }
        baseSortingOrders = new int[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            baseSortingOrders[i] = spriteRenderers[i].sortingOrder;
        }
        hpBar.rectTransform.localScale *= 1.1f;
        changeSkin();
        changeMaterial();
        updateHpBarImage();

    }
    public virtual void Update()
    {
        //更新
        switch (entityState)
        {
            case EntityState.disable:
                disableUpdate();
                break;
            case EntityState.enable:
                enableUpdate();
                break;
        }
    }
    /// <summary>
    /// disable状态的更新
    /// </summary>
    public virtual void disableUpdate()
    {
    }
    /// <summary>
    /// enable状态的更新
    /// </summary>
    public virtual void enableUpdate()
    {
        if (GameManager.Instance.gameMode == GameMode.SRCS) infiniteModeUpdate();//是否尸潮如水模式
        //是否攻击的时候定时爆炸
        if(GameManager.Instance.gameMode == GameMode.MiniGame_WGWE)
        {
            if (anim.GetBool("attacking"))
            {
                bombTimer += Time.deltaTime;
                if(bombTimer > 1.5f)
                {
                    bombTimer = 0;
                    var bomb = Instantiate(Utils.findEffectByType(AreaEffectType.BombWGWE), this.getEntityBoxColliderPos(), Quaternion.identity).GetComponent<BombEffect>();
                    bomb.entity = this;
                }
            }
        }
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.server != null)
        {
            syncTimer += Time.deltaTime;
            if(syncTimer >= 0.5f)
            {
                serverSyncEntity();
                syncTimer = 0;
            }
        }
        if(startFindAimTimerAfterDeploy < startFindAimTimeAfterDeploy)
        {
            startFindAimTimerAfterDeploy += Time.deltaTime;
        }
        if (buffList.Count != 0 && hitpoint <= 0) BuffManager.Instance.clearBuff(this);
        entityWalk();
        hpBarUpdate();
        gainBarUpdate();
        summonEntityUpdate();
        changeSkin();

        //自然随时间死亡
        if (lifeTime != 0 && !hasParent && (GameManager.Instance.gameMode == GameMode.Normal || (GameManager.Instance.gameMode == GameMode.MultiPlayer && entityType != EntityType.SkeletonTomb)) && !TestManager.Instance.建筑不随时间死亡)
        {
            changeHitpoint((maxHitpoint / lifeTime) * Time.deltaTime,null,null,true);
        }

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            return;
        }
        Vector2 colliderPos2 = boxCollider.bounds.center;
        float yPos = colliderPos2.y;
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i].gameObject.tag == "Shadow") continue;
            spriteRenderers[i].sortingLayerName = "Entity";
            int dynamicOrder = baseSortingOrders[i] - Mathf.RoundToInt(yPos * orderOffset);
            spriteRenderers[i].sortingOrder = dynamicOrder + (moveSpeed != 0 ? 10 : 0);
        }
    }
    /// <summary>
    /// 转为disable状态
    /// </summary>
    public virtual void transitionToDisable() 
    {
        entityState = EntityState.disable;
        agent.enabled = false;
        entityShadow.SetActive(false);
        if (fixAnim)
        {
            this.fixAnimator();
        }
        else
        {
            anim.enabled = false;
        }
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.gameObject.tag == "Shadow") continue;
            spriteRenderer.sortingLayerName = "Other";
        }
    }
    /// <summary>
    /// 转为enable状态
    /// </summary>
    public virtual void transitionToEnable()
    {
        entityState = EntityState.enable;
        anim.enabled = true;
        entityShadow.SetActive(true);
        deployShadow.SetActive(false);
        agent.enabled = true;
    }
    /// <summary>
    /// 用于掉血、加血
    /// </summary>
    /// <param name="damage">血量变化/伤害</param>
    /// <returns>操作后实体是否已死亡</returns>
    public virtual bool changeHitpoint(float damage,Bullet damageBullet = null,Entity damager = null,bool deltaTimeDamage = false)
    {
        //扣血
        hitpoint -= damage;
        if (MultiGameManager.client == null || deltaTimeDamage) updateHpBar();
        //根据反伤伤害进行反伤
        if (damager != null && (reflectDamage != 0 || reflectBuffType != BuffType.none))
        {
            if (!damager.hasParent && damager.hitpoint > 0)
            {
                BuffManager.Instance.addBuff(damager, reflectBuffTime, reflectBuffType);
                damager.changeHitpoint(reflectDamage);
            }
        }
        //防御塔植物
        if (hasParent)
        {
            if (parentEntity.tag == "Tower")
            {
                var tower = parentEntity.GetComponent<Tower>();
                if (GameManager.Instance.gameMode == GameMode.SRCS) DialogKingShopFix.Instance.updateFixUI();
                tower.towerHp = hitpoint;
                if (tower.towerHp <= 0)
                {
                    tower.towerBreak();
                }
            }
        }
        //防止血量溢出
        if (hitpoint > maxHitpoint)
        {
            hitpoint = maxHitpoint;
        }
        //服务端同步血量
        if(!deltaTimeDamage && GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.server != null)
        {
            NetworkServerService.getUserById(1).Send(new PlayServerSyncEntityHitpoint(entityID, hitpoint));
        }
        //判定死亡
        if (hitpoint <= 0)
        {
            if (MultiGameManager.client != null && GameManager.Instance.gameMode == GameMode.MultiPlayer) return false;
            if (tag != "Home")
            {
                //化敌为友模式,不会播放死亡动画,直接转换阵营
                if (GameManager.Instance.gameMode == GameMode.MiniGame_TETF)
                {
                    changeEntityGroup();
                    return false;
                }
                else
                {
                    hpBar.gameObject.SetActive(false);
                    if (agent.isOnNavMesh) agent.isStopped = true;
                    agent.speed = 0;
                    agent.radius = 0.001f;
                    agent.height = 0.001f;
                    agent.enabled = false;
                    anim.SetBool("Die", true);
                    boxCollider.enabled = false;
                }
                return true;
            }
            else entityDie();
            return true;
        }
        //如果死亡则为TRUE
        return false;
    }
    /// <summary>
    /// 实体死亡
    /// </summary>
    public virtual void entityDie()
    {
        //golden的实体掉落奖励
        if (GameManager.Instance.gameMode == GameMode.SRCS && golden)
        {
            //有卡牌,掉金币
            if (CardManager.Instance.hasCard(entityType))
            {
                var item = BridgeManager.Instance.itemsOfCoin[1];
                Utils.giveCoinBag(item, (int)Mathf.Floor(maxHitpoint), this.getEntityBoxColliderPos());
            }
            //没金币,掉卡牌
            else
            {
                Utils.gift(BridgeManager.Instance.itemsOfCard.findGiftCard(entityType), this.getEntityBoxColliderPos());
            }
        }
        //服务器同步实体的死亡
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            if (MultiGameManager.server != null)
            {
                NetworkServerService.getUserById(1).Send(new PlayServerEntityDie(entityID));
            }
            if(MultiGameManager.client != null)
            {
                MultiGameManager.client.Send(new PlayClientEntityDie(entityID));
            }
        }
        //随机掉金币
        if (GameManager.Instance.inGame && GameManager.Instance.gameMode != GameMode.MultiPlayer)
        {
            if(entityGroup == EntityGroup.enemy)
            {
                int temp;
                temp = UnityEngine.Random.Range(1, 20);
                if (temp == 2)
                {
                    //Utils.summonCoin(this.getEntityBoxColliderPos(), 1);
                }
            }
        }
        if(gameObject != null) Destroy(gameObject);
    }
    /// <summary>
    /// 血条更新
    /// </summary>
    public virtual void updateHpBar()
    {
        if (hpBar != null && hpBarInner != null)
        {
            hpBarInner.fillAmount = hitpoint / maxHitpoint;
        }
    }
    /// <summary>
    /// 更新血条图标
    /// </summary>
    public virtual void updateHpBarImage()
    {
        if (!hasParent)
        {
            var bars = ImageManager.Instance.hpBarImages;
            if (bars.Count > 0)
            {
                hpBarInner.GetComponent<Image>().sprite = bars[entityGroup.isFriend() ? 2 : 3];
                hpBar.GetComponent<Image>().sprite = bars[1];
            }
        }
    }
    /// <summary>
    /// 根据阵营切换皮肤
    /// </summary>
    public virtual void changeSkin()
    {
        foreach (var gameObject in changeGroupSkin)
        {
            if (entityGroup == EntityGroup.enemy)
            {
                if (gameObject.GetComponent<SpriteRenderer>().sprite == null) continue;
                gameObject.GetComponent<SpriteRenderer>().sprite = redSkin[changeGroupSkin.IndexOf(gameObject)];
            }
            else if (entityGroup == EntityGroup.friend)
            {
                if (gameObject.GetComponent<SpriteRenderer>().sprite == null) continue;
                gameObject.GetComponent<SpriteRenderer>().sprite = blueSkin[changeGroupSkin.IndexOf(gameObject)];
            }
        }
    }
    /// <summary>
    /// 根据地图切换材质(用于仿制黑暗效果)
    /// </summary>
    public virtual void changeMaterial()
    {
        if (Utils.inNight())
        {
            foreach (var sp in gameObject.getAllSR())
            {
                sp.material = BridgeManager.Instance.inDarkMaterial;
            }
        }
    }
    /// <summary>
    /// 找最近(第二近)的实体目标
    /// </summary>
    public virtual Entity findingClosestAim()
    {
        if (entityState == EntityState.disable || sightRange == 0) return null;
        //多人游戏:客户端实体无权找目标
        if (MultiGameManager.server == null && GameManager.Instance.gameMode == GameMode.MultiPlayer) return null;
        if(startFindAimTimerAfterDeploy < startFindAimTimeAfterDeploy) return null;
        var closestDistance = 999f;
        Entity closestAim = null;

        var allEntities = Utils.findAllEntities();
        foreach (var obj in Utils.findAllTowerAndHome())
        {
            if (obj.GetComponent<Tower>() != null)
            {
                var towerE = obj.GetComponent<Tower>().attachEntity;
                allEntities.Add(towerE);
            }
            else if (obj.tag == "Home") allEntities.Add(obj.GetComponent<Home>());
        }
        foreach (var entity in allEntities)
        {
            if(entity == null) continue;

            if(ignoreAims != null)
            {
                if (ignoreAims.Count != 0)
                {
                    if (ignoreAims.Contains(entity)) continue;
                }
            }

            //法术释放体和队友无法成为追踪目标
            if(entity.isAreaEffect() || entity.entityGroup == entityGroup) continue;

            if(aim != null)
            {
                if (entity == aim) continue;
                if (GameManager.Instance.gameMode == GameMode.SRCS)
                {
                    if(aim == GameManager.Instance.home2)
                    {
                        if (entity == aim) continue;
                    }
                }
            }

            if (entity.hitpoint <= 0 || BuffManager.Instance.hasBuff(entity, BuffType.Bomb))
                continue;

            if(entity is AllAreaZombie)
            {
                if (entity.GetComponent<MinerZombie>().inMining) continue;
            }

            if (entity == this || entity.entityState == EntityState.disable || entity.entityGroup == entityGroup) continue;

            //坚果会成为优先攻击目标
            if (entity.entityType == EntityType.WallNut && entity.boxCollider.dist(boxCollider) <= range) return entity;

            float distance = Vector3.Distance(entity.transform.position, transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAim = entity;

                if (distance > sightRange)
                    continue;
            }
        }

        if (closestAim != null)
            return closestAim;

        if (hasParent)
            return null;

        var gm = GameManager.Instance;
        if (gm.enemyTowerCount > 0 && entityGroup == EntityGroup.friend)
        {
            if (GameManager.Instance.gameMode == GameMode.Normal || GameManager.Instance.gameMode == GameMode.MultiPlayer)
            {
                if (gm.tower3 == null && gm.tower4 != null) return gm.tower4.attachEntity;
                else if (gm.tower3 != null && gm.tower4 == null) return gm.tower3.attachEntity;
                else if(gm.tower3 == null && gm.tower4 == null && GameManager.Instance.level == 8) return null;
                else
                {
                    if (Vector3.Distance(gm.tower3.transform.position, transform.position) <= Vector3.Distance(gm.tower4.transform.position, transform.position))
                    {
                        return gm.tower3.attachEntity;
                    }
                    else
                    {
                        return gm.tower4.attachEntity;
                    }
                }
            }
            else return null;
        }
        else if (gm.friendTowerCount > 0 && entityGroup == EntityGroup.enemy)
        {
            if(gm.tower1 == null)return gm.tower2.attachEntity;
            if(gm.tower2 == null) return gm.tower1.attachEntity;
            if (Vector3.Distance(gm.tower1.transform.position, transform.position) <= Vector3.Distance(gm.tower2.transform.position, transform.position))
            {
                return gm.tower1.attachEntity;
            }
            else
            {
                return gm.tower2.attachEntity;
            }
        }
        else
        {
            if (entityGroup == EntityGroup.friend)
            {
                if (GameManager.Instance.gameMode == GameMode.Normal || GameManager.Instance.gameMode == GameMode.MultiPlayer) return gm.home2;
                else return null;
            }
            else
                return gm.home1;
        }
    }

    /// <summary>
    /// 走路,同时寻找目标,攻击
    /// </summary>
    public virtual void entityWalk()
    {
        if (hasParent || this.isPlant() || isLockPos) transform.position = lockPos;
        if (aim == null || aim.hitpoint <= 0) // 无目标或目标已死亡的，寻找新目标
        {
            if(GameManager.Instance.gameMode == GameMode.SRCS && GameManager.Instance.enemyCount <= 0)
            {
                if (GameManager.Instance.home2 == null) return;
                aim = GameManager.Instance.home2;
                return;
            }
            aimNoSure = findingClosestAim();
            if (aimNoSure != null)
            {
                hitTimer = 0;
                aim = aimNoSure;
            }
            return;
        }
        else
        {
            aimTimer += Time.deltaTime;
            if (aimTimer >= hitTime) // 延时选择目标
            {
                aimTimer = 0;
                aimNoSure = findingClosestAim();
                if (aimNoSure != null)
                {
                    if(aimNoSure.entityType == EntityType.WallNut && aim.entityType != EntityType.WallNut && aimNoSure.boxCollider.dist(boxCollider) <= range)
                    {
                        hitTimer = 0;
                        aim = aimNoSure;
                    }
                    else if (aimNoSure.boxCollider.dist(boxCollider) + 0.1f < aim.boxCollider.dist(boxCollider) && !anim.GetBool("attacking") && aim.entityType != EntityType.WallNut)
                    {
                        hitTimer = 0;
                        aim = aimNoSure;
                    }
                }
            }
        }
        if (aim == null || aim.hitpoint <= 0 || hitpoint <= 0 || BuffManager.Instance.hasBuff(aim, BuffType.Bomb) || aim.entityGroup == entityGroup)
        {
            aim = findingClosestAim();
            hitTimer = 0;
            anim.SetBool("attacking", false);
        }
        if(hitpoint > 0)
        {
            float scaleX = transform.localScale.x;
            if (aim == null) return;
            if (!hasParent && !this.isZombie())
                scaleX = Math.Abs(transform.localScale.x) * (transform.position.x - aim.transform.position.x < 0 ? 1 : -1);
            if (this.isZombie())
                scaleX = Math.Abs(transform.localScale.x) * (transform.position.x - aim.transform.position.x < 0 ? -1 : 1);

            if (scaleX != transform.localScale.x)
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
        hitTimer += Time.deltaTime;
        if (aim == null) return;
        var distanceToAim = aim.boxCollider.dist(boxCollider);
        if(aim == GameManager.Instance.home2 && GameManager.Instance.enemyCount > 0 && GameManager.Instance.gameMode == GameMode.SRCS)//防止老吃房子
        {
            aimNoSure = findingClosestAim();
            if (aimNoSure != null)
            {
                hitTimer = 0;
                anim.SetBool("attacking", false);
                if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
                aim = aimNoSure;
            }
            return;
        }
        if (hitTimer >= hitTime)
        {
            if (distanceToAim <= range && hitpoint > 0 && aim.entityGroup != entityGroup)
            {
                if (aim.entityGroup == entityGroup)
                {
                    aimNoSure = findingClosestAim();
                    if (aimNoSure != null)
                    {
                        hitTimer = 0;
                        anim.SetBool("attacking", false);
                        if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
                        aim = aimNoSure;
                    }
                    return;
                }
                bool attackSoundFlag = true;

                if (MultiGameManager.client == null || moveSpeed != 0)
                {
                    anim.SetTrigger("attack");
                    anim.SetBool("attacking", true);

                    //服务端让客户端实体攻击
                    if (MultiGameManager.server != null)
                    {
                        NetworkServerService.getUserById(1).Send(new PlayServerLetEntityShoot(entityID));
                    }
                }

                if (bullet == null && GetComponent<GiantZombie>() == null && !noTimerAttack)
                {
                    bool dead = attack(damage,null,this);
                    if (dead)
                    {
                        if (entityType.isZombie() && !aim.entityType.isZombie())
                        {
                            attackSoundFlag = false;
                            Sounds.gulp.playWithPitch(UnityEngine.Random.Range(0.75f, 1.1f));
                        }
                        anim.SetBool("attacking", false);
                        aim = null;
                        hitTimer = 0;
                        if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
                        return;
                    }
                }
                if (attackSoundFlag) attackSounds.playWithPitch();
                hitTimer = 0;
            }
        }
        if (distanceToAim > range)
        {
            if (anim.GetBool("attacking") && (MultiGameManager.client == null || moveSpeed != 0)) anim.SetBool("attacking", false);

            if (!hasParent && moveSpeed != 0)
            {
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(aim.getEntityBoxColliderPos());
                    agent.isStopped = false;
                }
            }
        }
        else
        {
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
        }
        if (aim.hitpoint <= 0) aim = null;
    }
    public virtual bool attack(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        return aim.changeHitpoint(damage, damageBullet, damager, deltaTimeDamage);
    }
    /// <summary>
    /// 发射子弹
    /// </summary>
    /// <param name="bullet">子弹，不能为 null</param>
    public virtual bool shoot(Bullet bullet = null)
    {
        if (aim == null) return false;
        if (bullet == null)
        {
            if (this.bullet == null) return false;
            bullet = this.bullet;
        }

        bullet.shooter = this;
        processShotBullet(Instantiate(bullet.gameObject, transform.position, Quaternion.identity).GetComponent<Bullet>());
        return true;
    }
    /// <summary>
    /// 处理射出的子弹
    /// </summary>
    public virtual void processShotBullet(Bullet shotBullet)
    {
        var shootPos = transform.position;
        if (hasParent)
        {
            shootPos.x += bulletOffsetX * (parentEntity.transform.localScale.x >= 0 ? -1 : 1);
        }
        else
        {
            shootPos.x += (transform.localScale.x >= 0 ? bulletOffsetX : bulletOffsetX * -1f);
        }
        shootPos.y += bulletOffsetY;
        shotBullet.transform.position = shootPos;
        shotBullet.direction = (aim.getEntityBoxColliderPos() - shootPos) / (aim.getEntityBoxColliderPos() - shootPos).magnitude;
        shotBullet.flyRange = range * (shotBullet.flyMode == 1 ? 1 : 2);
    }
    /// <summary>
    /// 血条跟随
    /// </summary>
    public virtual void hpBarUpdate()
    {
        if (hasParent) return;
        if (!DataManager.Instance.data.showHpBar || BuffManager.Instance.hasBuff(this, BuffType.Bomb) || hitpoint <= 0 || (!onSpriteMask && Utils.inNight()) || !GameManager.Instance.inGame)
        {
            hpBar.gameObject.SetActive(false);
            return;
        }
        hpBar.gameObject.SetActive(true);
        var offsetPos = transform.position;

        if (tag != "Home" && !this.isZombie())//是不是植物
        {
            offsetPos.x += 0.4f;
        }

        else if (tag == "Home") offsetPos.x += ((float)((float)Screen.width/ (float)Screen.height) < (1920f/1080f) ? (entityGroup == EntityGroup.friend ? 1f : -1f) : (entityGroup == EntityGroup.friend ? 0.01f : -0.01f));

        else if (this.isZombie()) offsetPos.x += 0.3f;

        else offsetPos.x += (parentEntity.transform.localScale.x >= 0 ? 0.5f : -0.5f);

        offsetPos.y += hpBaroffsetY; offsetPos.x += hpBaroffsetX;
        Utils.uiMove(hpBar.gameObject, offsetPos);
    }
    /// <summary>
    /// 实体生成到格子上时的事件
    /// </summary>
    /// <param name="cell"></param>
    public virtual void addToCellEvent(Cell cell)
    {

    }
    /// <summary>
    /// 点击卡牌生成实体后的事件
    /// </summary>
    public virtual void useCardEvent()
    {

    }
    /// <summary>
    /// 生成实体update
    /// </summary>
    public virtual void summonEntityUpdate()
    {
        if(!summonEntity) return;
        summonTimer += Time.deltaTime;
        if(summonTimer >= summonDuartion / (golden ? 2 : 1))
        {
            summonTimer = 0;
            summon_Entity();
        }
    }
    /// <summary>
    /// 生成实体
    /// </summary>
    public virtual void summon_Entity()
    {
    }
    /// <summary>
    /// 转换阵营:被策反
    /// </summary>
    public virtual void changeEntityGroup(bool keepFullOfHp = true)
    {
        //防御塔和家不能被转换
        if (hasParent || tag == "Home") return;

        //法术实体直接去世
        if (this.isAreaEffect())
        {
            entityDie();
            return;
        }

        //满血
        if (keepFullOfHp)
        {
            hitpoint = maxHitpoint;
        }
        entityGroup = entityGroup == EntityGroup.friend ? EntityGroup.enemy : EntityGroup.friend;
        updateHpBarImage();
        updateHpBar();
        changeSkin();
        aim = null;
        aimNoSure = null;
        aimTimer = 0;
    }
    /// <summary>
    /// 碰撞体检测STAY
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Cell" && entityState == EntityState.enable)
        {
            cell = other.GetComponent<Cell>();
            if (this.isPlant())
            {
                if (hasParent && !cell.towerCell) return;
                cell.currentEntity = this;
            }
        }
        if (this.isAreaEffect()) return;
        if (Utils.inNight() && other.GetComponent<SpriteMask>() != null)
        {
            onSpriteMask = true;
        }
        else
        {
            onSpriteMask = false;
        }
    }
    /// <summary>
    /// 检测碰撞体离开
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit2D(Collider2D other)
    {
        if (this.isAreaEffect()) return;
        if (Utils.inNight() && other.GetComponent<SpriteMask>() != null)
        {
            onSpriteMask = false;
        }
    }
    #region 尸如潮水限定
    /// <summary>
    /// 收获事件
    /// </summary>
    private void gain()
    {
        if (hasParent) return;
        lifeTimer = -999;

        CoinManager.Instance.changeCoin(20 * (1 + (int)Math.Floor(DataManager.Instance.data.scoreSRCS / 10000)));
        Instantiate(Utils.findEffectByType(AreaEffectType.CoinSpread), transform.position, Quaternion.identity);
        Sounds.coin_click.play();
        entityDie();
    }
    /// <summary>
    /// 浇水事件
    /// </summary>
    public void watered()
    {
        changeHitpoint(-5);
        lifeTimer += 5;
        needingWater = false;
        Sounds.植物shoot.play();
        Utils.summonCoin(this.getEntityBoxColliderPos(), UnityEngine.Random.Range(1, 6));
    }
    /// <summary>
    /// 收获条跟随
    /// </summary>
    private void gainBarUpdate()
    {
        if (GameManager.Instance.gameMode == GameMode.SRCS)
        {
            if (canGains && moveSpeed == 0)
            {
                if (hasParent || tag == "Home") return;
                if (gainBar == null || gainBarInner == null) return;

                gainBarInner.fillAmount = lifeTimer / lifeTime;
                gainBarIcon.sprite = ImageManager.Instance.gainBarIcon[needingWater ? 1 : 0];
                Vector3 offsetPos = transform.position;
                offsetPos.y += hpBaroffsetY - 0.25f; offsetPos.x += hpBaroffsetX + 0.4f;
                Utils.uiMove(gainBar.gameObject, offsetPos);
                gainBar.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 根据模式设置属性
    /// </summary>
    public virtual void modeSet()
    {
        if (GameManager.Instance.gameMode == GameMode.SRCS)//尸潮如水
        {
            if (moveSpeed == 0)
            {
                lifeTime = 300;
                maxHitpoint *= 3;
                hitpoint = maxHitpoint;
                canGains = true;
                canWatering = true;
                needWaterDuration = needWaterDuration = UnityEngine.Random.Range(30, 60);
            }
            else
            {
                canGains = false;
                canWatering = false;
            }
        }
    }
    /// <summary>
    /// 尸如潮水更新状态
    /// </summary>
    private void infiniteModeUpdate()
    {
        if (canGains)
        {
            var delta = Time.deltaTime;
            lifeTimer += delta;

            if (lifeTimer >= lifeTime)
            {
                gain();
            }
        }
        if (canWatering && !needingWater)
        {
            needWaterTimer += Time.deltaTime;
            if (needWaterTimer >= needWaterDuration)
            {
                needingWater = true;
                needWaterTimer = 0;
                needWaterDuration = UnityEngine.Random.Range(60, 180);
            }
        }
    }
    #endregion

    #region 多人游戏限定
    /// <summary>
    /// 服务器发送同步的包,让客户端同步实体数据 默认发给1
    /// </summary>
    public void serverSyncEntity()
    {
        if (aim == null) return;
        NetworkServerService.getUserById(1).Send(new PlayServerSyncEntityAim(entityID, aim.entityID));
    }

    /// <summary>
    /// 多人联机 同步实体目标 一般是客户端需要做的
    /// </summary>
    /// <param name="aimId"></param>
    public void syncAim(int aimId)
    {
        aimNoSure = Utils.findEntityByIDMultiGame(aimId);
        aim = aimNoSure;
    }
    #endregion

}
