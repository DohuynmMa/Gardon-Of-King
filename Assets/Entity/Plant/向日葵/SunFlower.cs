using UnityEngine;
using Assets.Scripts.Utils;
public class SunFlower : Plant
{
    [Header("SunFlower:Plant")]
    public Sun sunPrefab;
    public Sun moonLightPrefab;
    public float sunCount;
    public float spawnSunDuration;
    private float spawnSunTimer;

    public override void Update()
    {
        base.Update();
        spawnSunTimer += Time.deltaTime;
        if(spawnSunTimer >= spawnSunDuration)
        {
            anim.SetTrigger("spawnSun");
            spawnSunTimer = 0;
        }
    }
    public void spawnSun()
    {
        if (sunPrefab == null) return;
        if (entityGroup == EntityGroup.friend)
        {
            Vector2 colliderPos2 = GetComponent<BoxCollider2D>().bounds.center;
            Vector3 colliderPos3 = new Vector3(colliderPos2.x, colliderPos2.y, transform.position.z);
            var inNight = Utils.inNight();
            var sun = Instantiate(inNight ? moonLightPrefab : sunPrefab, colliderPos3, Quaternion.identity);
            sun.sunCount = sunCount;
            Sounds.µã»÷Ñô¹â.playWithPitch(Random.Range(0.9f, 1.1f));
        }
        else SunManager.Instance.changeEnemySun(sunCount);
    }
    public override void addToCellEvent(Cell cell)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
    public override void useCardEvent()
    {
        transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
    }
}
