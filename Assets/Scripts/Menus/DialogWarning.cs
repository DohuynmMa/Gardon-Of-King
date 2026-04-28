using Assets.Scripts.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogWarning : MonoBehaviour, UIHelper
{
    public static DialogWarning Instance { get; private set; }
    public GameObject dialogUI;

    public TextMeshProUGUI showTXTUGUI;
    public string showText;
    public Color32 textColor = Color.white;
    public float fontSize = 18.1f;

    public TextMeshProUGUI showTXTUGUIButton;
    public string showTextButton = "确 定";
    public Color32 textColorButton = Color.white;
    public float fontSizeButton = 20f;
    public string GetName()
    {
        return "Warning";
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
    [Action("open")]
    public void openDialog()
    {
        if (dialogUI.activeSelf) return;
        dialogUI.SetActive(true);
        showTXTUGUI.text = showText;
        showTXTUGUI.fontSize = fontSize;
        showTXTUGUI.color = textColor;

        showTXTUGUIButton.text = showTextButton;
        showTXTUGUIButton.fontSize = fontSizeButton;
        showTXTUGUIButton.color = textColorButton;
    }
    [Action("close")]
    public void closeDialog()
    {
        dialogUI.SetActive(false);
        clickEvent(showTXTUGUIButton.text);
        showTXTUGUI.text = "";
        showTXTUGUIButton.text = "确 定";
        fontSize = 18.1f;
        fontSizeButton = 20f;
        textColor = Color.white;
        textColorButton = Color.white;
    }
    [Action("coming soon")]
    public void comingSoon()
    {
        Utils.showDialog("暂未开启,敬请期待!", 18.1f, Color.white, "确 定", 20f, Color.white);
    }
    [Action("not enough card")]
    public void notEnoughCard()
    {
        Utils.showDialog("集齐8张卡牌后开启!\r\n(试用卡不计)\r\n当前进度:( " + DataManager.Instance.data.cardCount + " / 8 )", 18.1f, Color.white, "确 定", 20f, Color.white);
    }
    private void clickEvent(string s)
    {
        switch(s)
        {
            case "退 出":
                if (MultiGameManager.server != null)
                {
                    MultiGameManager.server.Stop();
                    MultiGameManager.server = null;
                }
                if (MultiGameManager.client != null)
                {
                    MultiGameManager.client.CloseAsync();
                    MultiGameManager.client = null;
                }
                UIManager.Instance.PFReRegisterUI();
                SceneManager.LoadScene("GameMenu");
                break;
        }
    }
}
