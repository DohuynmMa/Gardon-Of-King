using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DialogWallet : MonoBehaviour, UIHelper
{
    public static DialogWallet Instance { get; private set; }
    public GameObject coinUI;
    public TextMeshProUGUI changeCount;
    public TextMeshProUGUI coinCount;
    public string GetName()
    {
        return "Wallet";
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        UIManager.registerUI(this, true);
    }
    [Action("show wallet")]
    public void showWallet()
    {
        coinUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(726, -483), 1f);
        coinCount.text = DataManager.Instance.data.coinCount.ToString();
        changeCount.text = "";
    }
    [Action("hide wallet")]
    public void hideWallet()
    {
        coinUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1500, -483), 1f);
    }
}
