using UnityEngine;
using DG.Tweening;

public class FallingItemEffect : AreaEffect
{
    public float fallingHeight;
    public override void onSpawn()
    {
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x, transform.position.y + fallingHeight * 0.4f, transform.position.z), new Vector3(transform.position.x, transform.position.y - fallingHeight, transform.position.z) }, 1.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        base.onSpawn();
    }
}
