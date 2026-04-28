using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogTextbook : MonoBehaviour, UIHelper
{
    public static DialogTextbook Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }

    public GameObject textbookButton;//进入按钮
    public GameObject textbookUI;
    public GameObject textbookBg;
    private int textbookTemp;
    public bool inTextbook;
    public string GetName()
    {
        return "Textbook";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    [Action("open")]
    public void openTextbook()
    {
        findPaper();
        Sounds.paper.play();
        inTextbook = true;
        textbookUI.SetActive(true);
        Time.timeScale = 0f;
        textbookTemp = 0;
        updateTextbook();
    }
    [Action("close")]
    public void closeTextbook()
    {
        inTextbook = false;
        textbookUI.SetActive(false);
        Time.timeScale = DataManager.Instance.data.gameSpeed;
    }
    public void updateTextbook()
    {
        textbookBg.GetComponent<Image>().sprite = ImageManager.Instance.textbookBGs[textbookTemp];
    }
    [Action("next")]
    public void next()
    {
        textbookTemp = (textbookTemp + 1) % ImageManager.Instance.textbookBGs.Count;
        updateTextbook();
    }
    [Action("last")]
    public void last()
    {
        textbookTemp = (textbookTemp - 1 + ImageManager.Instance.textbookBGs.Count) % ImageManager.Instance.textbookBGs.Count;
        updateTextbook();
    }
    /// <summary>
    /// 根据模式寻找对应教程
    /// </summary>
    private void findPaper()
    {
        var im = ImageManager.Instance;
        var gm = GameManager.Instance;
        if (gm.gameMode == GameMode.Normal)
        {
            switch (gm.chapter)
            {
                case 1:
                    im.textbookBGs = im.textbookC1;
                    break;
                case 2:
                    im.textbookBGs = im.textbookC2;
                    break;
            }
        }
        else if(gm.gameMode == GameMode.SRCS)
        {
            im.textbookBGs = im.textbookSRCS;
        }
    }
}
