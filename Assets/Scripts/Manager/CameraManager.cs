using UnityEngine;
using DG.Tweening;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    public void shake(float shakeValue)
    {
        Vector3 pos = transform.position;
        transform.DOPath(new Vector3[] { transform.position, new Vector3(Random.Range(-shakeValue,shakeValue), Random.Range(-0.1f, 0.1f), transform.position.z) }, 0.05f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOPath(new Vector3[] { transform.position, new Vector3(Random.Range(-shakeValue, shakeValue), Random.Range(-0.1f, 0.1f), transform.position.z) }, 0.05f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.DOPath(new Vector3[] { transform.position, new Vector3(Random.Range(-shakeValue, shakeValue), Random.Range(-0.1f, 0.1f), transform.position.z) }, 0.05f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    transform.DOPath(new Vector3[] { transform.position, pos }, 0.05f, PathType.CatmullRom).SetEase(Ease.OutQuad);
                });
            });
        });
    }
}
