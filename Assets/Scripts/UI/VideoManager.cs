using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : SingletonMonobehaviour<VideoManager>
{
    [Header("视频播放参数")]
    [Tooltip("视频播放组件")]
    public VideoPlayer videoPlayer;
    [Tooltip("视频播放的画布")]
    public Transform canvas;

    /// <summary>
    /// 播放视频
    /// </summary>
    public void PlayVideo(VideoClip videoClip)
    {
        canvas.gameObject.SetActive(true);
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
    }

    /// <summary>
    /// 停止并关闭视频
    /// </summary>
    public void StopVideo()
    {
        canvas.gameObject.SetActive(false);
        videoPlayer.Stop();
    }

    /// <summary>
    /// 暂停视频
    /// </summary>
    public void PauseVideo()
    {
        videoPlayer.Pause();
    }
}
