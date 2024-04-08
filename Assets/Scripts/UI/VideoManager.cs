using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class VideoManager : SingletonMonobehaviour<VideoManager>
{
    [Header("视频播放参数")]
    [Space(5)]
    [Header("UI对象")]
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

    [Space(10)]
    [Header("过场图文演出参数")]
    [Space(5)]
    [Header("UI对象")]
    [Tooltip("过场图文演出UI面板")]
    public Transform cutSceneUIPanel;
    [Tooltip("过场图文演出图片UI对象")]
    public Image graphic;
    [Tooltip("过场图文演出字体UI对象")]
    public TMP_Text tmpText;
    private Queue<string> textForShow = new Queue<string>();

    /// <summary>
    /// 展示过场演出图片文本
    /// </summary>
    public void ShowCutScene(GraphicsAndText graphicsAndText)
    {
        var rows = graphicsAndText.text.text.Split("\n");
        foreach (var row in rows)
        {
            textForShow.Enqueue(row);
        }

        graphic.sprite = graphicsAndText.graphic;
        graphic.SetNativeSize();
        tmpText.text = textForShow.Dequeue();

        cutSceneUIPanel.gameObject.SetActive(true);
    }

    private void Update() {
        if (!cutSceneUIPanel.gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textForShow.Count > 0)
            {
                tmpText.text = textForShow.Dequeue();
            }
            else
            {
                cutSceneUIPanel.gameObject.SetActive(false);
            }
        }
    }
}
