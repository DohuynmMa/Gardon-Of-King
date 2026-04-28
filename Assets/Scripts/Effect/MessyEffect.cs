using DG.Tweening;
using UnityEngine;

public class MessyEffect : AreaEffect
{
    public Vector3 direction;
    public int count;
    public float minScale;
    public float maxScale;
    public bool colorful;
    public override void Update()
    {
        base.Update();
        transform.Translate(direction * Time.deltaTime);
    }
    public override void onSpawn()
    {
        base.onSpawn();
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        direction = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        if (minScale != maxScale) transform.localScale *= Random.Range(minScale, maxScale);
        if (colorful)
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            GetComponent<SpriteRenderer>().color = new Color(r, g, b, 1f);
        }
    }
}
