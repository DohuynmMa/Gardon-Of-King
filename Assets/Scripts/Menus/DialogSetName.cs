using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogSetName : MonoBehaviour, UIHelper
{
    public static DialogSetName Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public DialogMainMenu main { get => DialogMainMenu.Instance; }

    public GameObject setName;//新手取名
    public TextMeshProUGUI srk;//输入框名字txt
    public TextMeshProUGUI srk2;//输入框名字txt(没输)
    public string GetName()
    {
        return "SetName";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    public bool checkName()
    {
        if (main.xuanzhangjie != null) UIManager.fadeOutAllChild(main.xuanzhangjie);
        if (main.chapt1 != null) UIManager.fadeOutAllChild(main.chapt1);
        if (main.chapt2 != null) UIManager.fadeOutAllChild(main.chapt2);
        if (main.setCard != null) UIManager.fadeOutAllChild(main.setCard);
        if (main.pvpUI != null) UIManager.fadeOutAllChild(main.pvpUI);
        if (DataManager.Instance == null) SceneManager.LoadScene("Loading");
        if (DataManager.Instance.data.playerName == "")
        {
            main.sb.SetActive(false);
            Sounds.paper.play();
            setName.SetActive(true);
            return false;
        }
        else
        {
            return true;
        }
    }
    [Action("confirm name")]
    /// <summary>
    /// 确认名字
    /// </summary>
    public void confirmName()
    {
        if (srk.text.Length > 30)
        {
            return;
        }
        if (srk.text.Length < 2)
        {
            srk2.text = "名称太短";
            return;
        }
        DataManager.Instance.data.playerName = srk.text;
        DataManager.Instance.savePlayerData();
        DialogMainMenu.Instance.transitionToChoose();
    }
}
