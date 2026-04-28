using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sun : MonoBehaviour
{
    public float sunCount;
    public float collectTime = 0.6f;
    private void Start()
    {
        transform.DOPath(new Vector3[] { DialogGamingMenu.Instance.cardAreaSun.transform.position }, collectTime, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(()=> {
            SunManager.Instance.changeSun(sunCount);
            Destroy(gameObject);
        });
    }

}
