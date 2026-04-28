using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [Header("游戏正式发布前请确保全部设置为false")]
    public bool 快进开始游戏前木牌动画;
    public bool 取消准备好开始动画;
    public bool 显示测试用关卡;
    public bool 自动快进对话;
    public bool Delete键重置存档;
    public bool 跳过刚开始游戏的加载界面;
    public bool 不读取编辑器专用存档;
    public bool 按3时停;
    public float 时停倍数;
    public bool 隐藏聊天信息;
    public bool 建筑不随时间死亡;
    [Header("测试关卡设置")]
    public List<Card> 测试关卡AI卡组;
    public int 测试关卡难度系数;
    public HomeNpcType 测试关卡敌方守护者 = HomeNpcType.none;
    public EntityType 测试关卡防御塔植物 = EntityType.PeaShooter;
    public int 测试关卡最高阳光数 = 10;
    public int 测试关卡开始时阳光数 = 7;
    public float 测试关卡阳光增长倍数 = 1;
    public bool videoMode = false;
}
