using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tips : MonoBehaviour
{
    [SerializeField]private int temp;
    private float changeTimer;
    public List<Sprite> tipSprites;
    private SpriteRenderer sp;
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        temp = 0;
    }
    private void Update()
    {
        changeTimer += Time.deltaTime;
        if(changeTimer >= 10)
        {
            changeTimer = 0;
            changeSprite();
        }
    }
    private void OnMouseDown()
    {
        changeTimer = 0;
        changeSprite();
    }
    private void changeSprite()
    {
        temp = (temp + 1) % tipSprites.Count;
        updateSprite();
    }
    private void updateSprite()
    {
        sp.sprite = tipSprites[temp];
    }
}
