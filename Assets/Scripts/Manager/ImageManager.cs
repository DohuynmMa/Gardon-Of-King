using System.Collections.Generic;
using UnityEngine;

enum setTowerEntity
{
    豌豆射手 = 0,
    向日葵 = 1,
    坚果 = 2,
    卷心菜 = 3,
    玉米投手 = 4,
    寒冰射手 = 5,
    西瓜投手 = 6,
    机枪射手 = 7,
    糯米豌豆 = 8
}
public class ImageManager : MonoBehaviour
{
    public static ImageManager Instance { get; private set; }
    [Header("Will Load")]
    public List<Sprite> hpBarImages;//血条
    public List<Sprite> hpBarIcon;//图标
    public List<Sprite> gainBarIcon;//图标
    public List<Sprite> cantPlace;//不可摆放
    public List<Sprite> kingShopBG;//包含不同等级图标外观(尸如潮水)
    public List<Sprite> towerSkins;//包含不同阵营不同等级塔外观(尸如潮水)
    public List<Sprite> levelIcon;//包含不同等级图标外观(尸如潮水)
    public List<Sprite> crowns;
    public List<Sprite> alarms;
    [Header("Won't Load")]
    public List<Sprite> textbookBGs;//游戏内教程
    public List<Sprite> textbookC1;
    public List<Sprite> textbookC2;
    public List<Sprite> textbookSRCS;
    public List<Sprite> shopInfo;//菜单商店商品信息
    public List<Sprite> menuBg;//菜单背景 0:闯关 1:联机 2:益智游戏
    public GameObject jiesuan;//结算
    public List<Sprite> setTowerEntityPlant;
    public List<Sprite> homeNpc;
    public List<Sprite> homeNpcHead;
    private void Awake()
    {
        Instance = this;
        syncImage();
    }
    /// <summary>
    /// 同步图片数据
    /// </summary>
    public void syncImage()
    {
        var bm = BridgeManager.Instance;

        hpBarImages = bm.hpBarImages;
        hpBarIcon = bm.hpBarIcon;
        gainBarIcon = bm.gainBarIcon;
        cantPlace = bm.cantPlace;
        kingShopBG = bm.kingShopBG;
        towerSkins = bm.towerSkins;
        levelIcon = bm.levelIcon;
        crowns = bm.crowns;
        alarms = bm.alarms;
    }
}
