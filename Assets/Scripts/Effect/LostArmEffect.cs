using UnityEngine;
using DG.Tweening;

public class LostArmEffect : AreaEffect
{
    public bool groupMode;//会随队伍换色
    public Sprite[] red;
    public Sprite[] blue;
    public Entity ownEntity;
    public override void onSpawn()
    {
        base.onSpawn();
        if (groupMode)//换色
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var obj = transform.GetChild(i).gameObject;
                if (ownEntity.entityGroup == EntityGroup.friend)
                {
                    obj.GetComponent<SpriteRenderer>().sprite = blue[i];
                }
                else obj.GetComponent<SpriteRenderer>().sprite = red[i];
            }
        }
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z) }, 0.3f, PathType.CatmullRom).SetEase(Ease.OutQuad);
    }
}
