using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsManager : MonoBehaviour
{
    [Header("Will Load")]
    public List<AudioClip> bgms;
    public List<AudioClip> sounds;
    public List<AudioClip> voices;
    public static SoundsManager Instance { get; private set; }
    private Dictionary<string, AudioSource> soundSources = new Dictionary<string, AudioSource>();
    private AudioSource soundSource;
    private float musicTime = 0;
    private AudioSource musicSource;
    private AudioSource voiceSource;
    private AudioSource getSoundSource(float pitch)
    {
        AudioSource source;
        if (soundSources.TryGetValue(string.Format("{0:N2}", pitch), out source))
        {
            return source;
        }
        return soundSource;
    }
    void Awake()
    {
        Instance = this;
        musicSource = GetComponent<AudioSource>();
        if (soundSources.Count == 0)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            obj.name = "SoundManager";
            for (float i = 0.5f; i < 1.5; i += 0.01f)
            {
                var key = string.Format("{0:N2}", i);
                var source = obj.AddComponent<AudioSource>();
                source.loop = false;
                source.pitch = i;
                soundSources.Add(key, source);
            }
            soundSource = obj.AddComponent<AudioSource>();
            soundSource.loop = false;
            voiceSource = obj.AddComponent<AudioSource>();
            voiceSource.loop = false;
        }
        syncAudio();
    }
    void Start()
    {
        updateSettingVolume();
        if (Utils.scene == "GameMenu")
        {
            Musics.主菜单BGM.play(true); //主菜单bgm
        }
    }
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量大小 (范围0-1)</param>
    public static void setSoundVolume(float volume)
    {
        Instance.soundSource.volume = volume;
        foreach (var source in Instance.soundSources.Values)
        {
            source.volume = volume;
        }
        DataManager.Instance.data.soundVolume = volume;
    }
    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">音量大小 (范围0-1)</param>
    public static void setMusicVolume(float volume)
    {
        Instance.musicSource.volume = volume;
        DataManager.Instance.data.musicVolume = volume;
    }
    /// <summary>
    /// 设置语音音量
    /// </summary>
    /// <param name="volume">音量大小 (范围0-1)</param>
    public static void setVoiceVolume(float volume)
    {
        Instance.voiceSource.volume = volume;
        DataManager.Instance.data.voiceVolume = volume;
    }
    public static void stopMusic()
    {
        Instance.musicSource.Stop();
    }
    public static void pauseMusic()
    {
        Instance.musicTime = Instance.musicSource.time;
        Instance.musicSource.Stop();
    }
    public static void resumeMusic()
    {
        Instance.musicSource.time = Instance.musicTime;
        Instance.musicSource.Play();
    }
    public static void playMusic(int id, bool loop,float speed)
    {
        var music = Instance.bgms[id];
        Instance.musicTime = 0;
        Instance.musicSource.clip = music;
        Instance.musicSource.loop = loop;
        Instance.musicSource.pitch = 1.0f + (speed - 1.0f) * 0.5f;
        Instance.musicSource.Play();
    }
    public static void playMusic(int id, bool loop)
    {
        var music = Instance.bgms[id];
        Instance.musicTime = 0;
        Instance.musicSource.clip = music;
        Instance.musicSource.loop = loop;
        Instance.musicSource.Play();
    }
    public static void playVoice(int id)
    {
        var voice = Instance.voices[id];
        Instance.voiceSource.clip = voice;
        Instance.voiceSource.loop = false;
        Instance.voiceSource.Play();
    }
    public static void playSounds(int id)
    {
        var music = Instance.sounds[id];
        Instance.soundSource.pitch = 1.0f;
        Instance.soundSource.PlayOneShot(music);
    }
    public static void playSounds(int id,float volume)//调整音量
    {
        var music = Instance.sounds[id];
        Instance.soundSource.pitch = 1.0f;
        Instance.soundSource.PlayOneShot(music,DataManager.Instance.data.soundVolume * volume);
    }
    public static void playSounds(int id, float volume, float speed)
    {
        var music = Instance.sounds[id];
        Instance.soundSource.pitch = speed;
        Instance.soundSource.PlayOneShot(music, DataManager.Instance.data.soundVolume * volume);
    }
    public static void playSoundsWithPitch(int id)
    {
        playSoundsWithPitch(id, UnityEngine.Random.Range(0.75f, 1.25f));
    }
    public static void playSoundsWithPitch(int id, float pitch)
    {
        AudioClip music = Instance.sounds[id];
        var source = Instance.getSoundSource(pitch);
        source.PlayOneShot(music);
    }
    public static void playSoundsWithPitch(int id, float pitch,float volume)
    {
        AudioClip music = Instance.sounds[id];
        var source = Instance.getSoundSource(pitch);
        source.PlayOneShot(music, DataManager.Instance.data.soundVolume * volume);
    }
    private void updateSettingVolume()
    {
        DataManager.Instance.loadPlayerData();
        var data = DataManager.Instance.data;

        setMusicVolume(data.musicVolume);
        setSoundVolume(data.soundVolume);
        setVoiceVolume(data.voiceVolume);
    }
    public void syncAudio()
    {
        var bm = BridgeManager.Instance;
        if (bm == null)
        {
            SceneManager.LoadScene("Loading");
        }

        bgms = bm.bgms;
        sounds = bm.sounds;
        voices = bm.voices;
    }
}
