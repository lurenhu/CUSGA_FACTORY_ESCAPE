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
    public TMP_Text tmpText;
    [Tooltip("过场图文演出的导演组件")]
    public Animator animator;
    [Header("文字播放参数")]
    [Tooltip("每个文字播放之间的间隔时间")]
    public float playingTimeInterval;

    private string currentAnimationState = Setting.stringDefaultValue;
    private int animationIndex = 0; 
    private List<CutSceneCell> cutSceneCellList = new List<CutSceneCell>();
    private Queue<string> textForShow = new Queue<string>();

    private bool textFinished;// 判断当前文本是否结束
    private bool cancelTyping;// 判断是否取消打字

    /// <summary>
    /// 获取所有的图片与对应的文本内容，并开始图文演出
    /// </summary>
    public void ShowCutScenes(List<CutSceneCell> cutSceneCellList)
    {
        RestoreInitialState();

        this.cutSceneCellList = cutSceneCellList;

        ShowCutScene(cutSceneCellList[animationIndex]);

        DialogSystem.Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取并排列好文本,并初始化文本与图片内容
    /// </summary>
    private void ShowCutScene(CutSceneCell cutSceneCell)
    {
        var rows = cutSceneCell.text.text.Split("\n");
        foreach (var row in rows)
        {
            textForShow.Enqueue(row);
        }

        ChangeAnimation(cutSceneCell.animationStateName);
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
                animationIndex++;
                if (animationIndex >= cutSceneCellList.Count)
                {
                    cutSceneUIPanel.gameObject.SetActive(false);
                    DialogSystem.Instance.gameObject.SetActive(true);
                    return;
                }
                ShowCutScene(cutSceneCellList[animationIndex]);
            }
        }
    }

    /// <summary>
    /// 启动文字逐一播放
    /// </summary>
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

    /// <summary>
    /// 恢复初始状态
    /// </summary>
    private void RestoreInitialState()
    {
        animationIndex = 0;
        cutSceneCellList.Clear();
        textForShow.Clear();
        cutSceneUIPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 切换动画状态
    /// </summary>
    private void ChangeAnimation(string animationStateName, float crossFade = 0.2f)
    {
        if (currentAnimationState != animationStateName)
        {
            currentAnimationState = animationStateName;
            animator.CrossFade(animationStateName,crossFade);
        }
    }
}
