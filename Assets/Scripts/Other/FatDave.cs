using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
public class FatDave : MonoBehaviour
{
    public Animator anim;
    public GameObject mouthObj;
    public float maxHunger = 200;
    public float hunger = 0;

    public int belchTemp = 0;

    public Image hungerBarInner;
    public GameObject hungerBar;
    public bool openMouth;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        hungerBar.SetActive(false);
        hunger = 0;
        transform.position = new Vector3(-5.6f,10,0);
        transform.DOPath(new Vector3[] { new Vector3(-5.6f, 0, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.InQuad).OnComplete(() =>
        {
            CameraManager.Instance.shake(0.5f);
            Sounds.‘“±Ò.playWithPitch();
            Instantiate(Utils.findEffectByType(AreaEffectType.GlassBroken));
            Instantiate(Utils.findEffectByType(AreaEffectType.GroundBrokenEffect),new Vector3(-5.75f,-0.5f,0),Quaternion.identity);
            GameManager.Instance.tower1.towerBreak();
            GameManager.Instance.tower2.towerBreak();
            GameManager.Instance.tower3.towerBreak();
            GameManager.Instance.tower4.towerBreak();
            updateHungerBar();
            hungerBar.SetActive(true);
        });
    }
    private void Update()
    {
        mouthObj.SetActive(inChew() || openMouth);
    }
    public bool needFoods()
    {
        return hunger < maxHunger;
    }
    public void beFed(float hunger)
    {
        updateHungerBar();
        this.hunger += hunger;
        Sounds.≥‘.playWithPitch();
        belchTemp++;
        transform.localScale = Vector3.one * (1 + (float)(this.hunger / maxHunger) * 0.5f);
        if(belchTemp >= 10)
        {
            belchTemp = 0;
            anim.SetTrigger("belch");
        }
        else
        {
            anim.SetTrigger("chew");
        }
        if (!needFoods())
        {
            GameManager.Instance.gameOver(EntityGroup.friend);
            Destroy(gameObject);
        }
    }
    private void updateHungerBar()
    {
        hungerBarInner.fillAmount = (float)(hunger / maxHunger);
    }
    private bool inChew()
    {
        return anim.GetCurrentAnimatorStateInfo(1).IsName("FatDaveChew");
    }
    private void belchStart()
    {
        openMouth = true;
        Sounds.¥Ú‡√.playWithPitch();
        Instantiate(Utils.findEffectByType(AreaEffectType.DaveBelchEffect), mouthObj.transform.position, Quaternion.identity);
        Instantiate(Utils.findEffectByType(AreaEffectType.FlyingLeavesAndFlowersEffect));
    }
    private void belchEnd()
    {
        openMouth = false;
    }
    private void belchByAnim()
    {
        CameraManager.Instance.shake(0.15f);
        foreach (var e in Utils.findAllEntitiesByGroup(EntityGroup.enemy))
        {
            var pos = e.transform.position;
            pos.x += Random.Range(0.2f, .5f);
            e.transform.position = pos;
            e.changeHitpoint(5 * (1 + (float)(hunger / maxHunger)));
        }
    }
}
