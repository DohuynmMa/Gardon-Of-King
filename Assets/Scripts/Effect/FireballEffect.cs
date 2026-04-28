using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class FireballEffect : AreaEffect
{
    public GameObject car;
    public Entity parentEntity;
    public override void onSpawn()
    {
        base.onSpawn();
        car.transform.position = new Vector3(Random.Range(-14f, 14f), 30, 0);
        var direction = (transform.position - car.transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        car.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        car.transform.DOPath(new Vector3[] { transform.position }, 3, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
        {
            var bomb = Instantiate(Utils.findEffectByType(AreaEffectType.BigBomb),transform.position, Quaternion.identity).GetComponent<BombEffect>();
            bomb.entity = parentEntity;
            bomb.damage = 40;
        });
    }
}
