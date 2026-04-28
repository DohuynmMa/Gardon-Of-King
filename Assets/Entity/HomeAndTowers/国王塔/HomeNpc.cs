using UnityEngine;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
using static UnityEngine.EventSystems.EventTrigger;
public enum HomeNpcType
{
    none,
    Dave
}
public class HomeNpc : MonoBehaviour
{
    public HomeNpcType homeNpcType = HomeNpcType.none;
    public Home home;//房门

    public Entity currentAim;//当前目标

    public Home_Target target;//准心

    public float shootDuration;
    private float shootTimer;

    private float syncAimTimer = 0.4f;//多人游戏同步目标
    private void Start()
    {
        //判定是否为夜晚章节
        if(Utils.inNight())
        {
            foreach (var spriteRenderer in gameObject.getAllSR())
            {
                spriteRenderer.material = BridgeManager.Instance.inDarkMaterial;
            }
        }
    }

    public void npcUpdate()
    {
        switch (homeNpcType)
        {
            case HomeNpcType.none:
                return;
            case HomeNpcType.Dave:
                daveUpdate();
                break;
        }
    }
    private void daveUpdate()
    {
        shootTimer += Time.deltaTime;
        aimingToAimToShoot();
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
        sync();
    }
    private Entity findingAim()
    {
        //多人游戏:客户端这边实体无权找目标
        if (MultiGameManager.server == null && GameManager.Instance.gameMode == GameMode.MultiPlayer) return null;

        float shootRange = home.GetComponent<Entity>().range;
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

            if (entityObject == gameObject || entity.entityState == EntityState.disable || entity.entityGroup == home.entityGroup)
                continue;

            if (entity.tag == "Home" || entity.hasParent || entity.entityGroup == home.entityGroup || entity.isAreaEffect())
                continue;


            if (entity.GetComponent<MinerZombie>() != null)
            {
                if (entity.GetComponent<MinerZombie>().inMining) continue;
            }

            var distance = Vector3.Distance(entity.getEntityBoxColliderPos(), home.transform.position);

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
    /// <summary>
    /// 多人游戏同步目标
    /// </summary>
    public void syncAim(int entityId)
    {
        var entity = Utils.findEntityByIDMultiGame(entityId);
        if (entity == null) return;
        currentAim = entity;
    }
    /// <summary>
    /// 多人游戏同步区域
    /// </summary>
    public void sync()
    {
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer || MultiGameManager.server == null) return;

        syncAimTimer += Time.deltaTime;
        if (syncAimTimer >= 0.5f && currentAim != null)
        {
            //发送目标默认为1
            NetworkServerService.getUserById(1).Send(new PlayServerSyncHomeNpcAim(home.GetComponent<Entity>().entityID, currentAim.entityID));
            syncAimTimer = 0;
        }
    }
    private void shoot()
    {
        if(currentAim == null || currentAim.entityGroup == home.entityGroup)
        {
            return;
        }
        CameraManager.Instance.shake(-0.2f);
        var aimPos = currentAim.getEntityBoxColliderPos();
        Instantiate(Utils.findEffectByType(AreaEffectType.ZombieBlood), aimPos, Quaternion.identity);
        currentAim.changeHitpoint(10);
        currentAim = null;
        target.isAimed = false;
    }
    private void playSound(int ID)//动画
    {
        SoundsManager.playSounds(ID);
    }
    private void playVoice(int ID)//动画
    {
        SoundsManager.playVoice(ID);
    }
}
