using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogResetData : MonoBehaviour, UIHelper
{
    public static DialogResetData Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }

    public GameObject deleteDataUI;
    public TMP_InputField 确认删除Field;
    public bool resettingData = false;
    public string GetName()
    {
        return "ResetData";
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
    public void openResetConfirmUI()
    {
        deleteDataUI.SetActive(true);
        resettingData = true;
    }
    [Action("close")]
    public void closeResetConfirmUI()
    {
        deleteDataUI.SetActive(false);
        resettingData = false;
    }
    [Action("reset")]
    public void restData()
    {
        if (!resettingData || 确认删除Field.text != "确认") return;

        DataManager.Instance.resetPlayerData();
        DOVirtual.DelayedCall(1.5f, () =>
        {
            DialogMainMenu.Instance.close();
        });
    }
}
