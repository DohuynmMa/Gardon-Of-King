using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Assets.Scripts.NetWork.Packet;
using Assets.Scripts.Utils;
using System;
using System.Threading;

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance;
    Queue<Action> _wait = new Queue<Action>();
    double _lock;
    bool _run;
    //需要传输的变量
    [Header("Bridge")]
    public int chapter;
    public int level;
    public GameMode gameMode;
    public BackgroundType backgroundType;
    public float sunPointAddSpeed;
    public float maxSunPoint;
    public float startSunPoint;
/// <summary>
/// 常用数据存储
/// </summary>

    [Header("Image")]
    public List<Sprite> hpBarImages;//血条
    public List<Sprite> hpBarIcon;//血条图标
    public List<Sprite> gainBarIcon;//收获条图标 0:不需要水,1:需要水

    public List<Sprite> cantPlace;//不可摆放区域

    public List<Sprite> kingShopBG;//包含不同等级图标外观(尸如潮水)
    public List<Sprite> towerSkins;//包含不同阵营不同等级塔外观(尸如潮水)
    public List<Sprite> levelIcon;//包含不同等级图标外观(尸如潮水)

    public List<Sprite> crowns;//塔爆炸图标的爆炸粒子
    public List<Sprite> alarms;//加速图标

    [Header("Audio")]
    public List<AudioClip> bgms;
    public List<AudioClip> sounds;
    public List<AudioClip> voices;

    [Header("Video")]
    public List<VideoClip> videos;

    [Header("Item")]
    public Item[] itemsOfCard;
    public Item[] itemsOfCoin;
    public Item[] itemsOfOthers;
    [Header("EntityPrefab")]
    public List<Entity> allEntityPrefab;
    [Header("Card")]
    public List<Card> allCards;
    [Header("Material")]
    public Material inDarkMaterial;
    [Header("Background")]
    public List<BackGround> backGroundPrefabs;
    [Header("Effect")]
    public List<AreaEffect> areaEffectPrefabs;
    [Header("DaveFood")]
    public List<FatDaveFood> daveFoodPrefabs;
    private void Awake()
    {
        PacketManager.init();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    private void Update()
    {
        //解决线程
        if (_run)
        {
            Queue<Action> execute = null;
            if (0 == Interlocked.Exchange(ref _lock, 1))
            {
                execute = new Queue<Action>(_wait.Count);
                while (_wait.Count != 0)
                {
                    Action action = _wait.Dequeue();
                    execute.Enqueue(action);
                }
                _run = false;
                Interlocked.Exchange(ref _lock, 0);
            }
            if (execute != null)
            {
                while (execute.Count != 0)
                {
                    Action action = execute.Dequeue();
                    action.Invoke();
                }
            }
        }
    }
    public void BeginInvoke(Action action)
    {
        while (true)
        {
            if (0 == Interlocked.Exchange(ref _lock, 1))
            {
                _wait.Enqueue(action);
                _run = true;
                Interlocked.Exchange(ref _lock, 0);
                break;
            }
        }
    }
}

