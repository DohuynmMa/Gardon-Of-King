using Assets.Scripts.Utils;
using UnityEngine;

public class Zombie : Entity
{
    [Header("Zombie")]
    public bool hasArm = true;
    public bool hasHead = true;
    public AreaEffectType loseHeadEffectType;
    public AreaEffectType loseArmEffectType;
    public Transform zombieHeadTransform;
    public Transform losingArmTransform;

    private float shoutTime;//ºð½Ð¼ä¸ô
    private float shoutTimer;
    public bool isShout;

    public override void Awake()
    {
        base.Awake();
        hasArm = true;
        hasHead = true;
    }
    public override void Start()
    {
        base.Start();
        shoutTime = Random.Range(6f, 12f);
    }

    public override void Update()
    {
        base.Update();
        if (hitpoint / maxHitpoint <= 0.5f && hasArm)
        {
            fallArm();
        }
        if (hitpoint <= 0 && hasHead)
        {
            hideHead();
        }
        if (isShout)
        {
            shoutTimer += Time.deltaTime;
            if (shoutTimer >= shoutTime && hitpoint > 0)
            {
                shoutTimer = 0;
                shoutTime = Random.Range(6f, 12f);
                shout();
            }
        }
    }
    public void playSound(int ID)//ANIMATION
    {
        SoundsManager.playSounds(ID);
    }
    public override void entityDie()
    {
        if(Random.Range(0,3) == 1 && entityType != EntityType.Skeleton && entityType != EntityType.SkeletonTomb) Instantiate(Utils.findDaveFoodByType(FatDaveFoodType.RottenFlesh),this.getEntityBoxColliderPos(),Quaternion.identity);
        base.entityDie();
    }
    public virtual void shout()//Ëæ»úÂÒ½Ð
    {
    }
    public virtual void fallArm()
    {
        if (!hasArm) return;
        hasArm = false;
        if (loseArmEffectType == AreaEffectType.None || losingArmTransform == null) return;
        var armEffectPrefab = Utils.findEffectByType(loseArmEffectType);
        if (armEffectPrefab == null)
        {
            print("null falling arm effect");
            return;
        }
        var fallingArm = Instantiate(armEffectPrefab, losingArmTransform.position, Quaternion.identity);
        fallingArm.transform.localScale = new Vector3(transform.localScale.x >= 0 ? 1 : -1, 1, 1);
        var armEff = fallingArm.GetComponent<LostArmEffect>();
        armEff.ownEntity = this;
    }
    public virtual void hideHead()
    {
        if (!hasHead) return;
        hasHead = false;
        if (loseHeadEffectType == AreaEffectType.None || zombieHeadTransform == null) return;
        var headEffectPrefab = Utils.findEffectByType(loseHeadEffectType);
        if (headEffectPrefab == null)
        {
            print("null falling head effect");
            return;
        }
        var fallingHead = Instantiate(headEffectPrefab, zombieHeadTransform.position, Quaternion.identity);
        fallingHead.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().flip = new Vector3(transform.position.x >= 0 ? 0 : 1, 0, 0);
    }

}
