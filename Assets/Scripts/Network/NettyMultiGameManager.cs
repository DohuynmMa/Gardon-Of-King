using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using Assets.Scripts.Utils;
using Assets.Scripts.Network.Procotol.Client.Login;
using Assets.Scripts.Network.Procotol;
using Assets.Scripts.Network;
using System.Net;
using System;
using Assets.Scripts;
using System.Threading.Tasks;
using Assets.Scripts.Network.Handlers;
public class NettyMultiGameManager : MonoBehaviour
{
    public static NettyService Netty { get; set; }
    public static DateTime StartTime { get; set; }
    public static IPAddress IPAddress { get; set; }
    public static int Port { get; set; }

    public static async void Initialize(string ip, int port)
    {

        StartTime = DateTime.UtcNow;
        IPAddress = IPAddress.Parse(ip);
        Port = port;
        Netty = new NettyService();
        await Task.Run(Netty.RunServerAsync);
    }

    /*public static void putEntityByPacket(Entity entity)
    {
        entity.transitionToDisable();

        //防具实体和护盾实体的处理
        if (entity.GetComponent<ArmorEntity>() != null)
        {
            if (entity.GetComponent<ArmorEntity>().hasArmor) entity.GetComponent<ArmorEntity>().armorHp = entity.GetComponent<ArmorEntity>().maxArmorHp;
        }
        if (entity.GetComponent<ShieldEntity>() != null)
        {
            if (entity.GetComponent<ShieldEntity>().hasShield) entity.GetComponent<ShieldEntity>().shieldHitpoint = entity.GetComponent<ShieldEntity>().shieldMaxHitpoint;
        }

        //根据阵营翻转
        if (entity.entityGroup == EntityGroup.enemy)
        {
            entity.transform.localScale = new Vector3(
                entity.transform.localScale.x * -1,
                entity.transform.localScale.y,
                entity.transform.localScale.z
            );
        }

        //这里会特殊处理一些人因犯低级错误导致的位置和大小偏移
        if (entity.GetComponent<SunFlower>() != null)
        {
            entity.transform.localScale = new Vector3(1, 1, 1);
        }

        //根据移动速度是否为0锁定位置
        if (entity.moveSpeed == 0)
        {
            entity.lockPos = entity.transform.position;
        }

        Sounds.种植物.playWithPitch(UnityEngine.Random.Range(0.9f, 1.1f));
        entity.updateHpBarImage();
        entity.transitionToEnable();
    }*/
}
