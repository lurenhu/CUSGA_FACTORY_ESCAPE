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
    [Header("文字播放参数")]
    [Tooltip("每个文字播放之间的间隔时间")]
    public float playingTimeInterval;

    private int graphIndex = 0; 
    private List<GraphicsAndText> graphicsAndTextList = new List<GraphicsAndText>();
    private Queue<string> textForShow = new Queue<string>();

    private bool textFinished;// 判断当前文本是否结束
    private bool cancelTyping;// 判断是否取消打字

    /// <summary>
    /// 获取所有的图片与对应的文本内容，并开始图文演出
    /// </summary>
    public void ShowCutScenes(List<GraphicsAndText> graphicsAndTextList)
    {
        this.graphicsAndTextList = graphicsAndTextList;

        ShowCutScene(graphicsAndTextList[graphIndex]);

        cutSceneUIPanel.gameObject.SetActive(true);
        DialogSystem.Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取并排列好文本,并初始化文本与图片内容
    /// </summary>
    private void ShowCutScene(GraphicsAndText graphicsAndText)
    {
        var rows = graphicsAndText.text.text.Split("\n");
        foreach (var row in rows)
        {
            textForShow.Enqueue(row);
        }

        graphic.sprite = graphicsAndText.graphic;
        graphic.SetNativeSize();
        StartCoroutine(PlayingRowText(textForShow.Dequeue()));
    }

    private void Update() {
        if (!cutSceneUIPanel.gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textFinished && textForShow.Count > 0)
            {
                StartCoroutine(PlayingRowText(textForShow.Dequeue()));
            }
            else if (!textFinished && !cancelTyping)
            {
                cancelTyping = true;
            }
            else if (textFinished && textForShow.Count == 0)
            {
                graphIndex++;
                if (graphIndex >= graphicsAndTextList.Count)
                {
                    cutSceneUIPanel.gameObject.SetActive(false);
                    DialogSystem.Instance.gameObject.SetActive(true);
                    return;
                }
                ShowCutScene(graphicsAndTextList[graphIndex]);
            }
        }
    }

    IEnumerator PlayingRowText(string textToPlay)
    {
        textFinished = false;
        tmpText.text = Setting.stringDefaultValue;
        int index = 0;
        while (!cancelTyping && index < textToPlay.Length-1)
        {
            tmpText.text += textToPlay[index];
            index++;
            yield return new WaitForSeconds(playingTimeInterval);
        }

        tmpText.text = textToPlay;
        cancelTyping = false;
        textFinished = true;
    }
}
