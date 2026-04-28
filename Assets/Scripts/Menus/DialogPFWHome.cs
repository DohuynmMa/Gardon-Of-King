using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogPFWHome : MonoBehaviour, UIHelper
{
    public static DialogPFWHome Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }

    public int homeNpcTemp;//0: none 1: Dave
    public GameObject homeNpc;
    public GameObject homeNpcHead;
    public GameObject homeNpcInfoUI;
    public TextMeshProUGUI homeNpcInfo;
    public TextMeshProUGUI homeNpcName;
    public GameObject 选择你的守护者;
    public string GetName()
    {
        return "PFWHome";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    public void updateHomeNpc()
    {
        switch (homeNpcTemp)
        {
            case 0:
                homeNpc.GetComponent<Animator>().enabled = false;
                DataManager.Instance.data.homeNpc = HomeNpcType.none;
                homeNpc.GetComponent<Image>().sprite = ImageManager.Instance.homeNpc[0];
                homeNpcHead.GetComponent<Image>().sprite = ImageManager.Instance.homeNpcHead[0];
                homeNpcInfo.text = "无";
                homeNpcName.text = "无";
                break;
            case 1:
                if (DataManager.Instance.data.hasDave)
                {
                    homeNpc.GetComponent<Animator>().enabled = true;
                    DataManager.Instance.data.homeNpc = HomeNpcType.Dave;
                    homeNpc.GetComponent<Image>().sprite = ImageManager.Instance.homeNpc[1];

                    homeNpcInfo.text = "使用狙击枪守护房门";
                }
                else
                {
                    homeNpc.GetComponent<Animator>().enabled = false;
                    homeNpc.GetComponent<Image>().sprite = ImageManager.Instance.homeNpc[2];
                    homeNpcInfo.text = "暂未获得";
                }
                homeNpcHead.GetComponent<Image>().sprite = ImageManager.Instance.homeNpcHead[1];
                homeNpcName.text = "疯狂戴夫";
                break;
        }
    }
    public void loadHomeNpcInfo()
    {
        switch (DataManager.Instance.data.homeNpc)
        {
            case HomeNpcType.none:
                homeNpcTemp = 0;
                homeNpc.GetComponent<Image>().sprite = ImageManager.Instance.homeNpc[0];
                homeNpcHead.GetComponent<Image>().sprite = ImageManager.Instance.homeNpcHead[0];
                homeNpcInfo.text = "无";
                homeNpcName.text = "无";
                break;
            case HomeNpcType.Dave:
                homeNpcTemp = 1;
                homeNpc.GetComponent<Image>().sprite = ImageManager.Instance.homeNpc[1];
                homeNpcHead.GetComponent<Image>().sprite = ImageManager.Instance.homeNpcHead[1];
                homeNpcInfo.text = "使用狙击枪守护房门";
                homeNpcName.text = "疯狂戴夫";
                break;
        }
    }
    [Action("next")]
    public void nextNPC()//BUTTON
    {
        homeNpcTemp = (homeNpcTemp + 1) % ((ImageManager.Instance.homeNpc.Count + 1) / 2);
        updateHomeNpc();
    }
    [Action("last")]
    public void lastNPC()//BUTTON
    {
        homeNpcTemp = (homeNpcTemp - 1 + ((ImageManager.Instance.homeNpc.Count + 1) / 2)) % ((ImageManager.Instance.homeNpc.Count + 1) / 2);
        updateHomeNpc();
    }
}
