using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
public class MultiGameManager : MonoBehaviour
{
    public static WebSocket client;
    public static WebSocketServer server;
    public static bool isPrepared;
    public static int myUserId;
    public static string enemyName;//对手名字
    public static int version = 4;
    public static void putEntityByPacket(Entity entity)
    {

        //根据阵营翻转
        if (entity.entityGroup == EntityGroup.enemy)
        {
            entity.transform.localScale = new Vector3(
                entity.transform.localScale.x * -1,
                entity.transform.localScale.y,
                entity.transform.localScale.z
            );
        }

        //根据移动速度是否为0锁定位置
        if (entity.isPlant())
        {
            entity.lockPos = entity.transform.position;
        }

        Sounds.种植物.playWithPitch(UnityEngine.Random.Range(0.9f, 1.1f));
        entity.updateHpBarImage();
        entity.transitionToEnable();
    }
}
