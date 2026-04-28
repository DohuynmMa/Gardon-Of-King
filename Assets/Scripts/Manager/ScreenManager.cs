using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    [Header("需要适配的Canvas(共用场景)")]
    public GameObject walletCanvas;
    private void Awake()
    {
        Instance = this;
    }
    public int lastWidth;
    public int lastHeight;
    float updateScreenTimer;
    float updateMessageScaleTimer;
    private void Start()
    {
        DataManager.Instance.loadPlayerData();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        setFullScreen();
        float 屏幕比例 = (float)((float)Screen.width / (float)Screen.height);
        walletCanvas.GetComponent<CanvasScaler>().screenMatchMode = 屏幕比例 < 16f / 9f ? CanvasScaler.ScreenMatchMode.Shrink : CanvasScaler.ScreenMatchMode.Expand;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (DialogConsole.Instance.inTyping) return;
            DataManager.Instance.data.fullScreen = !DataManager.Instance.data.fullScreen;
            DataManager.Instance.savePlayerData();
            setFullScreen();
        }
        if (!Application.isMobilePlatform && !Application.isEditor)
        {
            updateWidthAndHeight();
            updatingUpdateMessageObjScale();
        }
    }
    /// <summary>
    /// 根据设置自动全屏
    /// </summary>
    public void setFullScreen()
    {
        if (DataManager.Instance.data.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
    /// <summary>
    /// 保持16:9的窗口宽高比
    /// </summary>
    private void updateWidthAndHeight()
    {
        updateScreenTimer += Time.deltaTime;
        if (updateScreenTimer >= 0.3f)
        {
            if (Screen.width != lastWidth)
            {
                Screen.SetResolution(Screen.width, (int)(Screen.width * (9f / 16f)), DataManager.Instance.data.fullScreen);

            }
            else if (Screen.height != lastHeight)
            {
                Screen.SetResolution((int)(Screen.height * (16f / 9f)), Screen.height, DataManager.Instance.data.fullScreen);
            }
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            updateScreenTimer = 0;
        }
    }
    /// <summary>
    /// 消息UI与屏幕的适配(Update中)
    /// </summary>
    private void updatingUpdateMessageObjScale()
    {
        if (DataManager.Instance.data.fullScreen) return;
        updateMessageScaleTimer += Time.deltaTime;
        if (updateMessageScaleTimer >= 0.3f)
        {
            updateMessageObjScale();
            updateMessageScaleTimer = 0;
        }
    }
    /// <summary>
    /// 消息UI与屏幕的适配(直接调用)
    /// </summary>
    public void updateMessageObjScale()
    {
        var bl = (float)Screen.safeArea.height / (float)Screen.height;
        for (int i = 0; i < DialogConsole.Instance.messagePrefabs.Count; i++)
        {
            var obj = DialogConsole.Instance.messagePrefabs[i];
            if (obj == null) continue;
            var rec = obj.GetComponent<RectTransform>();
            if (rec == null) continue;
            rec.localScale = new Vector3(0.2695f, 0.2695f, 0.2695f) / bl;
        }
        for (int b = 0; b < ConsoleManager.Instance.showingMessages.Count; b++)
        {
            var obj = ConsoleManager.Instance.showingMessages[b];
            if (obj == null) continue;
            var rec = obj.GetComponent<RectTransform>();
            if (rec == null) continue;
            rec.localScale = new Vector3(1, 1, 1) / bl;
        }
    }
}
