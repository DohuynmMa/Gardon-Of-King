using UnityEngine;
public enum AreaEffectType
{
    None,
    ZombieBlood,
    CoinSpread,
    IceMelonAoe,
    MelonAoe,
    NpeaBulletBreak,
    NurlMeWater,
    PeaBulletBreak,
    SpreadingStar,
    TowerBreakBlue,
    FrozenAll,
    RottenPlant,
    MakeupZombieBomb,
    SmallBomb,
    BigBomb,
    PekkaBroken,
    GlassBroken,
    SnowPeaBulletBreak,
    CabbageBulletBreak,
    KernalBulletBreak,
    ButterBulletBreak,
    MelonBulletBreak,
    IceMelonBulletBreak,
    GemBulletBreak,
    TowerBreakRed,
    BasicZombieLoseHead,
    ImpZombieLoseHead,
    GoblinImpZombieLoseHead,
    PaperZombieNormalLoseHead,
    PaperZombieAngryLoseHead,
    MakeupZombieNormalLoseHead,
    MakeupZombieAngryLoseHead,
    MinerZombieLoseHead,
    Miner2ZombieLoseHead,
    PekkaLoseArm,
    NormalZombieLoseArm,
    ImpLoseArm,
    GoblinImpLoseArm,
    MakeupZombieNormalLoseArm,
    MakeupZombieAngryLoseArm,
    PaperZombieLoseArm,
    MinerZombieLoseArm,
    Miner2ZombieLoseArm,
    FallingCone,
    FallingBucket,
    FallingNewYearHat,
    FallingHardHat,
    FallingMinerHat,
    FallingBluePekkaHead,
    FallingNewspaper,
    FallingRedPekkaHead,
    FallingMakeupZombieHair,
    PekkaHitEffect,
    RageLiquid,
    LeavingSnow,
    Fireball,
    HealLiquid,
    PoisonLiquid,
    BombWGWE,
    GiantPekkaSpecialAttack,
    GiantPekkaError,
    GiantPekkaSpecialAoe,
    WitchZombieLoseHead,
    WitchZombieAoe,
    DaveBelchEffect,
    GroundBrokenEffect,
    FlyingLeavesAndFlowersEffect
}
public class AreaEffect : MonoBehaviour
{
    /// <summary>
    /// 是否随时间消失?(不常用,一般用粒子系统和动画机自动删除)
    /// </summary>
    public bool autoDie = true;
    public Sounds startSound = Sounds.无音效;
    public Sounds endSound = Sounds.无音效;
    public AreaEffectType type;
    /// <summary>
    /// 特效的存在时间
    /// </summary>
    public float lifeTime = 3f;
    public float lifeTimer;
    public bool isShake = false;
    public float shakePitch = 0;
    public Entity summonner;
    public virtual void Start()
    {
        onSpawn();
    }
    public virtual void Update()
    {
        if (autoDie)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer > lifeTime)
            {
                onEnd();
                lifeTimer = 0;
            }
        }
    }
    public virtual void endByAnim()
    {
        onEnd();
    }
    public virtual void onSpawn()
    {
        startSound.play();
        if (isShake) CameraManager.Instance.shake(shakePitch);
    }
    public virtual void onEnd()
    {
        endSound.play();
        Destroy(gameObject);
    }
}
