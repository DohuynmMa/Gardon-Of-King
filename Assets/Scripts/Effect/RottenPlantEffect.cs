using System.Collections.Generic;
using UnityEngine;

public class RottenPlantEffect : AreaEffect
{
    public List<Sprite> sprites;
    public EntityType entityType;
    public SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public override void Start()
    {
        base.Start();
        sr.sprite = null;
        switch (entityType)
        {
            case EntityType.PeaShooter:
                sr.sprite = sprites[0];
                break;
            case EntityType.SunFlower:
                sr.sprite = sprites[1];
                break;
            case EntityType.WallNut:
                sr.sprite = sprites[2];
                break;
            case EntityType.Cabbage:
                sr.sprite = sprites[3];
                break;
            case EntityType.Cornpult:
                sr.sprite = sprites[4];
                break;
            case EntityType.Watermelon:
                sr.sprite = sprites[5];
                break;
            case EntityType.IceMelon:
                sr.sprite = sprites[6];
                break;
            case EntityType.SnowPeaShooter:
                sr.sprite = sprites[7];
                break;
            case EntityType.Npeashooter:
                sr.sprite = sprites[8];
                break;
            case EntityType.GatlingPeaShooter:
                sr.sprite = sprites[9];
                break;
            case EntityType.LittleWolf:
                sr.sprite = sprites[10];
                break;
        }
        if(sr.sprite == null) Destroy(gameObject);
    }
}
