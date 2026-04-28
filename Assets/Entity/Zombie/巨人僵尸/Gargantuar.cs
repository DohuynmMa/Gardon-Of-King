using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class Gargantuar : GiantZombie
{
    private bool hurt1;
    private bool hurt2;
    public List<Sprite> weapons; // Œ‰∆˜Ã˘Õº±Ì
    public GameObject weapon; // Œ‰∆˜
    public List<GameObject> imp; // –°πÌ∆˜πŸ
    public override void Awake()
    {
        base.Awake();
        hasSon = true;
        hasThrowSon = false;
        hurt1 = false;
        hurt2 = false;
    }
    public override void Start()
    {
        base.Start();
        weapon.GetComponent<SpriteRenderer>().sprite = weapons[GameManager.Instance.gameMode == GameMode.MultiPlayer ? 0 : Random.Range(0, weapons.Count)];
        for (var i = 0; i < transform.GetChild(0).childCount; i++)
        {
            var childTransform = transform.GetChild(0).GetChild(i);
            if (childTransform.CompareTag("Gar_Imp"))
            {
                imp.Add(childTransform.gameObject);
            }
        }
    }
    public override void Update()
    {
        base.Update();
        if (!hasSon)
        {
            foreach (var obj in imp)
            {
                obj.SetActive(false);
            }
        }
    }
    private void dieShake()//À¿Õˆ“˝∆Camera’∂Ø
    {
        CameraManager.Instance.shake(0.1f);
        Sounds.‘“±Ò.playWithPitch();
    }
    private void startDie()//À¿Õˆ«∞Ω–…˘
    {
        Sounds.æﬁ»ÀΩ© ¨«˜ ∆.playWithPitch();
    }
    public override void fallArm()
    {
    }
    public override void hideHead()
    {
    }
    public override void updateSelf()
    {
        if (hitpoint <= maxHitpoint * 0.3f && !hurt2)
        {
            anim.SetTrigger("hurt2");
            hurt2 = true;
        }
        else if (hitpoint <= maxHitpoint * 0.6f && !hurt1!)
        {
            anim.SetTrigger("hurt1");
            hurt1 = true;
        }
        if (hitpoint <= maxHitpoint * 0.5f && !hasThrowSon)
        {
            hasThrowSon = true;
            throwSon();
        }
    }
    public override void throwSon()
    {
        if (hitpoint <= 0) return;
        DOVirtual.DelayedCall(0.6f, () =>
        {
            Sounds.∂™–°πÌ“Ù–ß.playWithPitch();
        });
        anim.SetTrigger("ThrowSon");
    }
    public override void attack()
    {
        if (aim == null) return;
        CameraManager.Instance.shake(0.1f);
        Sounds.‘“±Ò.playWithPitch();
        if (aim.hasParent || aim.tag == "Home") damage *= (1f / 3f);

        if (aim.changeHitpoint(damage, null, this))
        {
            if (aim.hasParent || aim.tag == "Home" || GameManager.Instance.gameMode == GameMode.MiniGame_TETF) return;
            var zabiePrefab = Utils.findEffectByType(AreaEffectType.RottenPlant).GetComponent<RottenPlantEffect>();
            zabiePrefab.entityType = aim.entityType;
            var zabieEffect = Instantiate(zabiePrefab.gameObject, aim.getEntityBoxColliderPos(), Quaternion.identity);
            Sounds.Ω© ¨≥‘.playWithPitch(Random.Range(0.75f, 0.9f));
        }
    }
}
