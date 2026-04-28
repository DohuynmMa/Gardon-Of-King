using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogPFWTower : MonoBehaviour, UIHelper
{
    public static DialogPFWTower Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }

    public GameObject towerEntityInfoUI;
    public GameObject towerEntity;//在塔上的植物
    public TextMeshProUGUI towerEntityInfo;
    public TextMeshProUGUI payPointCount;
    public string GetName()
    {
        return "PFWTower";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    /// <summary>
    /// 更新Tower的Entity立绘
    /// </summary>
    public void updateTowerEntity()
    {
        int temp = 0;
        switch (DataManager.Instance.data.towerEntity)
        {
            case EntityType.PeaShooter:
                temp = 0;
                towerEntityInfo.text = "可发射子弹攻击敌人";
                payPointCount.text = "0";
                break;
            case EntityType.SunFlower:
                temp = 1;
                towerEntityInfo.text = "可生产阳光";
                payPointCount.text = "1";
                break;
            case EntityType.WallNut:
                temp = 2;
                towerEntityInfo.text = "可减轻防御塔受到的伤害";
                payPointCount.text = "0";
                break;
            case EntityType.Cabbage:
                temp = 3;
                towerEntityInfo.text = "可抛射子弹攻击敌人";
                payPointCount.text = "0";
                break;
            case EntityType.Cornpult:
                temp = 4;
                towerEntityInfo.text = "可抛射子弹攻击敌人";
                payPointCount.text = "0";
                break;
            case EntityType.SnowPeaShooter:
                temp = 5;
                towerEntityInfo.text = "可发射子弹攻击敌人";
                payPointCount.text = "1";
                break;
            case EntityType.Watermelon:
                temp = 6;
                towerEntityInfo.text = "可抛射子弹攻击敌人";
                payPointCount.text = "2";
                break;
            case EntityType.GatlingPeaShooter:
                temp = 7;
                towerEntityInfo.text = "可发射子弹攻击敌人";
                payPointCount.text = "2";
                break;
            case EntityType.Npeashooter:
                temp = 8;
                towerEntityInfo.text = "可发射子弹攻击敌人";
                payPointCount.text = "1";
                break;
            case EntityType.IceMelon:
                temp = 9;
                towerEntityInfo.text = "可抛射子弹攻击敌人";
                payPointCount.text = "3";
                break;
            case EntityType.LittleWolf:
                temp = 10;
                towerEntityInfo.text = "可反伤并冰冻攻击者";
                payPointCount.text = "2";
                break;
            case EntityType.XbowPea:
                temp = 11;
                towerEntityInfo.text = "可发射子弹攻击敌人";
                payPointCount.text = "2";
                break;
            case EntityType.LIUDEHUA:
                temp = 12;
                towerEntityInfo.text = "快被破坏时可支援己方单位";
                payPointCount.text = "0";
                break;
        }
        towerEntity.GetComponent<Image>().sprite = ImageManager.Instance.setTowerEntityPlant[temp];
    }
}
