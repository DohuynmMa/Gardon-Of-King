using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }

    private VideoPlayer vp;
    public GameObject player;
    [Header("Will Load")]
    public List<VideoClip> videos;
    private void Awake()
    {
        Instance = this;
        vp = player.GetComponent<VideoPlayer>();
        syncVideo();
        player.SetActive(false);
    }
    /// <summary>
    /// 播放视频
    /// </summary>
    public void play(int ID)
    {
        SoundsManager.pauseMusic();
        VideoClip clip = videos[ID];
        player.SetActive(true);
        vp.clip = clip;
        vp.loopPointReached += VideoPlayer_loopPointReached;
        player.transform.SetAsLastSibling();
        vp.Play();
    }
    public float getTime(int ID)
    {
        VideoClip clip = videos[ID];
        return (float)clip.length;
    }
    /// <summary>
    /// 播放结束或播放到循环的点时被执行
    /// </summary>
    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        SoundsManager.resumeMusic();
        player.SetActive(false);
    }
    public void syncVideo()
    {
        var bm = BridgeManager.Instance;

        videos = bm.videos;
    }
}
