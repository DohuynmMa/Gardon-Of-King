using UnityEngine;
public enum BackgroundType
{
    Day,
    DayBoss,
    Night
}
public class BackGround : MonoBehaviour
{
    public GameObject cloud;
    public BlacknightMask blacknightMask;
    public BackgroundType backgroundType;
    private void Update()
    {
        if (cloud == null) return;

        //µ÷ÕûÔÆ
        Vector3 newSize = cloud.GetComponent<SpriteRenderer>().size;
        newSize.x += Time.deltaTime * 0.4f;
        if (newSize.x > 164.9) newSize.x = 40;
        cloud.GetComponent<SpriteRenderer>().size = newSize;
    }
}
