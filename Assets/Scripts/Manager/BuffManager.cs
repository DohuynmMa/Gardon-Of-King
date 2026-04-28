using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork;
using Assets.Scripts.NetWork.Server;
public enum BuffType
{
    none,
    Ice,
    NurlMe,
    Rage,
    Bomb,
    Buttering,
    IcePlus,
    Heal,
    Poison
}
public class BuffManager : MonoBehaviour
{
    private static List<Color> colors;
    private Dictionary<BuffType, Coroutine> buffCoroutines = new Dictionary<BuffType, Coroutine>();
    public static BuffManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        colors = buffColors();
    }
    /// <summary>
    /// Buff颜色大全
    /// </summary>
    /// <returns></returns>
    private static List<Color> buffColors() {
        List<Color> colors = new List<Color>();
        colors.Add(new Color(94f / 255f, 176f / 255f, 1)); // Ice 0
        colors.Add(new Color(224f / 255f, 1, 0)); // NurlMe 1
        colors.Add(new Color(211f / 255f, 0, 1)); // Rage 2
        colors.Add(new Color(0, 113f / 255f, 1)); // IcePlus 3
        colors.Add(new Color(138f/255f, 1, 0)); // Heal 4
        colors.Add(new Color(1, 188f / 255f, 0)); // Poison 5
        return colors;
    }
    public void addBuff(Entity addEntity,float buffTime,BuffType buffType,bool isReceivePacket = false)//给实体添加指定BUFF
    {
        //客户端无权加BUFF 等待服务端同步
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.client != null && !isReceivePacket) return;

        //服务端同步Buff
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.server != null)
        {
            //默认发给1
            NetworkServerService.getUserById(1).Send(new PlayServerAddBuffToEntity(addEntity.entityID, buffTime, buffType));
        }

        Buff buff = new Buff(buffType, buffTime,addEntity.GetInstanceID());
        if (addEntity.avoidingBuff.Contains(buffType))
        {
            return;
        }
        if(hasBuff(addEntity, buffType))
        {
            if (buffCoroutines.TryGetValue(buff.type, out Coroutine existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
                buffCoroutines.Remove(buff.type);
            }
            removeBuff(addEntity, buffType);
        }
        if(addEntity.GetComponent<MinerZombie>() != null)
        {
            if (addEntity.GetComponent<MinerZombie>().inMining) return;
        }
        addEntity.buffList.Add(buff);

        buffStart(addEntity, buff);

        Coroutine newCoroutine = StartCoroutine(waitBuffEnd(addEntity, buff));
        buffCoroutines[buff.type] = newCoroutine;
    }
    public void removeBuff(Entity removeEntity, BuffType buffType)//删除实体指定BUFF
    {
        for (int i = removeEntity.buffList.Count - 1; i >= 0; i--)
        {
            Buff buff = removeEntity.buffList[i];
            if (buff.type == buffType)
            {
                removeEntity.buffList.Remove(buff);
                buffEnd(removeEntity, buff.type);
                return;
            }
        }
    }
    public void clearBuff(Entity clearEntity)
    {
        for (int i = clearEntity.buffList.Count - 1; i >= 0; i--)
        {
            Buff buff = clearEntity.buffList[i];
            buffEnd(clearEntity, buff.type);
            clearEntity.buffList.Remove(buff);
        }
    }//清理实体全部BUFF
    public bool hasBuff(Entity hasEntity,BuffType buffType)
    {
        foreach (Buff buff in hasEntity.buffList)
        {
            if (buff.type == buffType)
            {
                return true;
            }
        }
        return false;
    }//是否有buff (Type)
    public Buff findBuff(Entity entity,BuffType buffType)
    {
        foreach (Buff buff in entity.buffList)
        {
            if (buff.type == buffType)
            {
                return buff;
            }
        }
        return null;
    }//寻找buff

    public IEnumerator<WaitForSeconds> waitBuffEnd(Entity entity,Buff buff)
    {
        yield return new WaitForSeconds(buff.buffDuration);
        removeBuff(entity,buff.type);
    }//等待buff结束
    public IEnumerator<WaitForSeconds> buffUpdatePerSecond(Entity entity, Buff buff)
    {
        yield return new WaitForSeconds(1);
        if (hasBuff(entity,buff.type))
        {
            switch (buff.type)
            {
                case BuffType.Ice:
                    changeColor(entity, colors[0]);
                    break;
                case BuffType.NurlMe:
                    changeColor(entity, colors[1]);
                    entity.changeHitpoint(-0.5f);
                    break;
                case BuffType.IcePlus:
                    changeColor(entity, colors[3]);
                    break;
                case BuffType.Heal:
                    entity.changeHitpoint(-1f);
                    changeColor(entity, colors[4]);
                    break;
                case BuffType.Poison:
                    entity.changeHitpoint(1f);
                    changeColor(entity, colors[5]);
                    break;
            }
            buff.buffDuration--;
            StartCoroutine(buffUpdatePerSecond(entity, buff));
        }
    }//buff每秒更新

    public void buffEnd(Entity entity, BuffType type)
    {
        switch (type)
        {
            case BuffType.Ice:
                entityTas(entity, 2);
                break;
            case BuffType.NurlMe:
                entityTas(entity, 1f / 1.5f);
                changeDamage(entity, 0.5f);
                changeMaxHitpoint(entity, 0.5f);
                changeRange(entity, 1f / 1.3f);
                break;
            case BuffType.Rage:
                entityTas(entity, 0.5f);
                break;
            case BuffType.Bomb:
                entity.hitpoint = 0;
                entity.entityDie();
                break;
            case BuffType.Buttering:
                entityTas(entity, 100f);
                break;
            case BuffType.IcePlus:
                entityTas(entity, 100f);
                break;
            case BuffType.Heal:
                break;
        }
        changeColor(entity, Color.white);
    }//buff结束时执行的代码
    private void buffStart(Entity entity , Buff buff)
    {
        switch (buff.type)
        {
            case BuffType.Ice:
                Sounds.冰冻音效.playWithPitch();
                entityTas(entity, 0.5f);
                changeColor(entity, colors[0]);
                break;
            case BuffType.NurlMe:
                entityTas(entity, 1.5f);
                changeDamage(entity, 2);
                changeMaxHitpoint(entity, 2);
                changeRange(entity, 1.3f);
                changeColor(entity, colors[1]);
                break;
            case BuffType.Rage:
                entityTas(entity, 2f);
                changeColor(entity, colors[2]);
                break;
            case BuffType.Bomb:
                changeColor(entity, Color.black);
                entityTas(entity, 0.01f);
                entity.agent.enabled = false;
                entity.sightRange = 0;
                entity.range = 0;
                changeDamage(entity, 0);
                entity.aim = null;
                break;
            case BuffType.Buttering:
                entityTas(entity, 0.01f);
                entity.aim = null;
                break;
            case BuffType.IcePlus:
                entityTas(entity, 0.01f);
                entity.aim = null;
                break;
            case BuffType.Heal:
                break;
        }
        StartCoroutine(buffUpdatePerSecond(entity, buff));
    }//buff开始时执行的代码

    private void entityTas(Entity entity,float time)//实体加减速(倍率)
    {
        if (entity == null) return;
        entity.moveSpeed *= time;
        if (entity.GetComponent<NavMeshAgent>() != null) entity.GetComponent<NavMeshAgent>().speed = entity.moveSpeed;

        if(time == 0)
        {
            entity.hitTime = 9999;
        }
        else entity.hitTime /= time;


        if (entity.anim != null)
        {
            entity.anim.speed *= time;
        }
    }
    private void changeMaxHitpoint(Entity entity,float time)//更改实体最高血量和现有血量(倍率)
    {
        entity.maxHitpoint *= time;
        entity.hitpoint *= time;
    }
    private void changeDamage(Entity entity,float time)//更改实体伤害(倍率)
    {
        entity.damage *= time;
    }
    private void changeRange(Entity entity,float time)//更改实体攻击距离(倍率)
    {
        entity.range *= time;
    }
    public void changeColor(Entity entity, Color color)//根据BUFF变色
    {
        if (entity == null || color == null) return;

        SpriteRenderer[] spriteRenderers = entity.gameObject.getAllSR();

        if (spriteRenderers == null) return;

        foreach (SpriteRenderer sp in spriteRenderers)
        {
            if (sp == null || sp.tag == "Shadow") continue;
            Color startColor = sp.color;
            for (int i = 0; i < 4; i++)
            {
                float delay = 0.1f * i;
                DOVirtual.DelayedCall(delay / DataManager.Instance.data.gameSpeed, () =>
                {
                    float t = delay / 0.4f;
                    sp.color = Color.Lerp(startColor, color, t);
                });
            }
        }
    }

}
