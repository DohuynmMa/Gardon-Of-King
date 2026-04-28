using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class GoblinGiant : GiantZombie
{
    private bool hurt1;
    private bool hurt2;
    public List<GameObject> imp;//小鬼器官
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
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            Transform childTransform = transform.GetChild(0).GetChild(i);
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
            foreach(GameObject obj in imp)
            {
                obj.SetActive(false);
            }
        }

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
            Sounds.丢小鬼音效.playWithPitch();
        });
        anim.SetTrigger("ThrowSon");
    }
    public override void attack()
    {
        if (aim == null) return;
        Sounds.哥布林戳人音效.playWithPitch();
        if (aim.hasParent || aim.tag == "Home") aim.changeHitpoint(damage);
        else
        {
            aim.changeHitpoint(damage,null,this);
            bool canPushback = aim.transform.position.x < 8f && aim.transform.position.x > -8f && aim.GetComponent<BossDave>() == null && aim.GetComponent<BossDave2>() == null && aim.canBePushback;
            if(canPushback) aim.transform.position = new Vector3(aim.transform.position.x + (transform.localScale.x >= 0 ? -0.5f : 0.5f), aim.transform.position.y, aim.transform.position.z);
            else Utils.checkAndFixEntityPos(aim);
        }
    }
    private void dieShake()//死亡引起Camera震动
    {
        CameraManager.Instance.shake(0.1f);
        Sounds.砸瘪.playWithPitch();
    }
    private void startDie()//死亡前叫声
    {
        Sounds.巨人僵尸趋势.playWithPitch();
    }
}
