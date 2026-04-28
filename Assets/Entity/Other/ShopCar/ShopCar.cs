using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;

public class ShopCard : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3(Random.Range(-12f, 12f), 18, 0);
        Vector3 direction = (Vector3.zero - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.DOPath(new Vector3[] { Vector3.zero}, 0.4f).SetEase(Ease.Linear).OnComplete(()=> { explosion(); });
    }
    private void explosion()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.BigBomb),transform.position,Quaternion.identity);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            Destroy(gameObject);
        });
        //todo 商品展现
    }
}
