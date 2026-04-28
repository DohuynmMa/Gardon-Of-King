using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using System;
using UnityEngine.UI;

public class SunManager : MonoBehaviour
{
    public float maxSunPoint;
    public float addTime;
    public Image sunMask;
    public static SunManager Instance { get; private set; }
    public DialogGamingMenu gaming { get => DialogGamingMenu.Instance; }
    public float sunPoint;
    public float enemySunPoint;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateSunPointText();
    }
    private void Update()
    {
        if (sunPoint >= maxSunPoint)
        {
            sunMask.fillAmount = 1;
            gaming.sunPointMaxText.text = "/Âú";
        }
        else
        {
            gaming.sunPointMaxText.text = "/" + maxSunPoint;
            changeSun(Time.deltaTime / (3f / addTime));
            sunMask.fillAmount = sunPoint - (float)Math.Floor(sunPoint);
        }
        if(enemySunPoint < maxSunPoint)
        {
            enemySunPoint += Time.deltaTime / (3f - AI_Manager.Instance.difficult * 0.2f);
        }
    }

    private void UpdateSunPointText()
    {
        gaming.sunPointText.text = Math.Floor(sunPoint).ToString();
        var dgm = DialogGamingMenu.Instance;
        sunMask.sprite = Utils.inNight() ? dgm.moonSprite : dgm.sunSprite;
    }
    public void changeSun(float point)
    {
        if(sunPoint + point > maxSunPoint)
        {
            sunPoint = maxSunPoint;
        }
        else sunPoint += point;
        UpdateSunPointText();
    }
    public void changeEnemySun(float point)
    {
        if (enemySunPoint + point > maxSunPoint)
        {
            enemySunPoint = maxSunPoint;
        }
        else enemySunPoint += point;
        UpdateSunPointText();
    }

}
