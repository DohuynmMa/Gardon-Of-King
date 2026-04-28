using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
/// <summary>
/// 游戏设置
/// </summary>
public class DialogSettings : MonoBehaviour, UIHelper
{
    public static DialogSettings Instance { get; private set; }
    public GameObject settingUI;
    public Scrollbar music;
    public Scrollbar sound;
    public Scrollbar voice;
    public Scrollbar speedBar;
    public TextMeshProUGUI speedBarText;

    public Toggle showHpBar;
    public Toggle setScreen;
    public Toggle skipVideo;

    [HideInInspector] public bool inSetting;

    public string GetName()
    {
        return "Settings";
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
    public void openSetting()
    {
        inSetting = true;
        music.value = DataManager.Instance.data.musicVolume;
        sound.value = DataManager.Instance.data.soundVolume;
        voice.value = DataManager.Instance.data.voiceVolume;
        speedBar.value = (DataManager.Instance.data.gameSpeed - 0.5f) / 1.5f;
        showHpBar.isOn = DataManager.Instance.data.showHpBar;
        setScreen.isOn = DataManager.Instance.data.fullScreen;
        skipVideo.isOn = DataManager.Instance.data.skipVideo;
        settingUI.SetActive(true);
    }

    [Action("close")]
    public void closeSetting()
    {
        inSetting = false;
        settingUI.SetActive(false);
        DataManager.Instance.savePlayerData();
    }
    [Action("music")]
    public void setMusicVolume()
    {
        float volume = music.value;
        SoundsManager.setMusicVolume(volume);
    }
    [Action("sound")]
    public void setSoundVolume()
    {
        float volume = sound.value;
        SoundsManager.setSoundVolume(volume);
    }
    [Action("voice")]
    public void setVoiceVolume()
    {
        float volume = voice.value;
        SoundsManager.setVoiceVolume(volume);
    }
    [Action("speed")]
    public void setGameSpeed()
    {
        float[] speeds = { 0.5f, 1.0f, 1.5f, 2.0f };

        int index = Mathf.RoundToInt(speedBar.value * (speeds.Length - 1));
        index = Mathf.Clamp(index, 0, speeds.Length - 1);
        speedBarText.text = speeds[index] + "x";
        DataManager.Instance.data.gameSpeed = speeds[index];
    }
    [Action("showHp")]
    public void setShowHpBar()
    {
        bool isShow = showHpBar.isOn;
        DataManager.Instance.data.showHpBar = isShow;
        DataManager.Instance.savePlayerData();
    }
    [Action("fullscreen")]
    public void setFullScreen()
    {
        bool isFull = setScreen.isOn;
        DataManager.Instance.data.fullScreen = isFull;
        DataManager.Instance.savePlayerData();
        ScreenManager.Instance.setFullScreen();
    }
    [Action("skip video")]
    public void setSkipVideo()
    {
        bool isSkip = skipVideo.isOn;
        DataManager.Instance.data.skipVideo = isSkip;
        DataManager.Instance.savePlayerData();
    }

}