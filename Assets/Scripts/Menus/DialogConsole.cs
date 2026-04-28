using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using WebSocketSharp;
public class DialogConsole : MonoBehaviour, UIHelper
{
    public static DialogConsole Instance { get; private set; }
    public ScreenManager sm { get => ScreenManager.Instance; }
    public GameObject showingMessages;
    public GameObject allMessages;
    public GameObject inputArea;
    public GameObject allMessageContent;
    public GameObject cellIdUI;
    public GameObject entitiesIdUi;
    public TMP_InputField inputTxt;
    public List<ConsoleMessage> messagePrefabs;
    public int sentMTemp;
    public bool inTyping = false;
    public string GetName()
    {
        return "Console";
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
        UIManager.registerUI(this,true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Slash))
        {
            if (Utils.scene == "GameMenu") return;
            if (DialogSettings.Instance.inSetting || Time.timeScale == 0 || inTyping) return;
            var inputString = Input.inputString;
            if (inputString == "/")
            {
                inputTxt.text = "/";
            }
            else
            {
                inputTxt.text = "";
            }
            openConsole();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && inTyping)
        {
            closeConsole();
        }
        if(Input.GetKeyDown(KeyCode.Return) && inTyping)
        {
            ConsoleManager.Instance.onSend(inputTxt.text);
            closeConsole();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && inTyping)
        {
            sentMTemp--;
            if (sentMTemp < 0)
            {
                sentMTemp = ConsoleManager.Instance.sentMessageList.Count - 1;
            }
            inputTxt.text = ConsoleManager.Instance.sentMessageList[sentMTemp];
            Canvas.ForceUpdateCanvases();
            inputTxt.caretPosition = inputTxt.text.Length;
            inputTxt.ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && inTyping)
        {
            sentMTemp++;
            if (sentMTemp > ConsoleManager.Instance.sentMessageList.Count - 1)
            {
                sentMTemp = ConsoleManager.Instance.sentMessageList.Count - 1;
            }
            inputTxt.text = ConsoleManager.Instance.sentMessageList[sentMTemp];
            Canvas.ForceUpdateCanvases();
            inputTxt.caretPosition = inputTxt.text.Length;
            inputTxt.ActivateInputField();
        }
        if (inTyping)
        {
            updateTypingTips();
        }
    }
    [Action("send")]
    public void sendByButton()
    {
        ConsoleManager.Instance.onSend(inputTxt.text);
        closeConsole();
    }
    [Action("open")]
    public void openConsole()
    {
        showingMessages.SetActive(false);
        allMessages.SetActive(true);
        inputArea.SetActive(true);
        messagePrefabs.Clear();
        Canvas.ForceUpdateCanvases();
        inputTxt.caretPosition = inputTxt.text.Length;
        inputTxt.ActivateInputField();
        inTyping = true;
        var ci = ConsoleManager.Instance;
        var parentRect = ci.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        for (var i = 0; i < ci.messages.Count; i++)
        {
            var m = Instantiate(ci.showingMessagePrefab);
            messagePrefabs.Add(m);
            m.transform.SetParent(parentRect);
            m.text.text = ci.messages[i];
            var screenBl = (float)Screen.safeArea.height / (float)Screen.height;
            var posX = parentRect.localPosition.x + 3f;
            var posY = (ci.messages.Count - i) * 14.9f  + parentRect.localPosition.y - 283.5f;
            m.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0) / screenBl;
        }
        allMessageContent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        sentMTemp = ConsoleManager.Instance.sentMessageList.Count;
        sm.updateMessageObjScale();
    }
    [Action("close")]
    public void closeConsole()
    {
        var t = TestManager.Instance;
        showingMessages.SetActive(!t.隐藏聊天信息);
        allMessages.SetActive(false);
        inputArea.SetActive(false);
        inTyping = false;
        var ci = ConsoleManager.Instance;
        var targetTransform = ci.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0);
        for (var i = targetTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(targetTransform.GetChild(i).gameObject);
        }
    }
    public void updateTypingTips()
    {
        if (!inputTxt.text.StartsWith("/s ") || GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            cellIdUI.SetActive(false);
            entitiesIdUi.SetActive(false);
            return;
        }
        var sts = inputTxt.text.Split(" ");
        switch (sts.Length)
        {
            case 2:
                entitiesIdUi.SetActive(true);
                cellIdUI.SetActive(false);
                break;
            case 3:
                cellIdUI.SetActive(!sts[1].IsNullOrEmpty());
                entitiesIdUi.SetActive(sts[1].IsNullOrEmpty());
                break;
            default:
                cellIdUI.SetActive(false);
                entitiesIdUi.SetActive(false);
                break;
        }
    }
}