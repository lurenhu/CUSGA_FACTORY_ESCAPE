using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VideoManager : SingletonMonobehaviour<VideoManager>
{
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

    [SerializeField] private string currentAnimationState = Setting.stringDefaultValue;
    private int animationIndex = 0; 
    private List<CutSceneCell> cutSceneCellList = new List<CutSceneCell>();
    private Queue<string> textForShow = new Queue<string>();

    private bool textFinished = true;// 判断当前文本是否结束
    private bool cancelTyping = false;// 判断是否取消打字

    [Header("连续播发过场动画")]
    private bool isPlayingAutoCutScene = false;
    public bool isPlayingCutScene = false;

    private void Update() {
        if (!cutSceneUIPanel.gameObject.activeSelf) return;

        if (Input.GetMouseButtonUp(0) && !isPlayingAutoCutScene)
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
                
                GetNextRowText();
            }
        }
        else if (isPlayingAutoCutScene)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // 动画播放结束
            if (stateInfo.normalizedTime >= 1f)
            {
                isPlayingAutoCutScene = false;

                animationIndex++;
                GetNextRowText();
            }
        }
    }

    /// <summary>
    /// 恢复初始状态
    /// </summary>
    private void RestoreInitialState()
    {
        animationIndex = 0;
        cutSceneCellList.Clear();
        textForShow.Clear();
    }

    /// <summary>
    /// 切换动画状态
    /// </summary>
    private void ChangeAnimation(CutSceneCell cutSceneCell)
    {
        string animationStateName = cutSceneCell.animationStateName;

        if (currentAnimationState != animationStateName)
        {
            currentAnimationState = animationStateName;
            animator.Play(animationStateName);
        }
    }

    /// <summary>
    /// 获取下一段文本
    /// </summary>
    private void GetNextRowText()
    {
        // 文本播放结束
        if (animationIndex >= cutSceneCellList.Count)
        {
            if (GameManager.Instance.gameState == GameState.Result)
            {
                GameManager.Instance.StartChangeSceneCoroutine("GameScene","MainMenu",GameState.Result);
                return;
            }
            else if (GameManager.Instance.gameState == GameState.Fail)
            {
                GameManager.Instance.StartChangeSceneCoroutine("GameScene","FailMenu",GameState.Fail);
                return;
            }
            else if (GameManager.Instance.gameState == GameState.Playing)
            {
                cutSceneUIPanel.gameObject.SetActive(false);
                UIManager.Instance.UIShow = false;

                isPlayingCutScene = false;
                
                GameManager.Instance.PlayCurrentLevelAudio();
                return;
            }
        }

        if (cutSceneCellList[animationIndex].isAuto)
        {
            ShowAutoCutScene(cutSceneCellList[animationIndex]);
        }
        else
        {
            ShowCutScene(cutSceneCellList[animationIndex]);
        }
    }


#region 播放图文动画(可交互)
    /// <summary>
    /// 获取所有的图片与对应的文本内容，并开始图文演出
    /// </summary>
    public void ShowCutScenes(List<CutSceneCell> cutSceneCellList)
    {
        if (cutSceneCellList.Count == 0) return;

        RestoreInitialState();

        this.cutSceneCellList = cutSceneCellList;
        cutSceneUIPanel.gameObject.SetActive(true);

        if (cutSceneCellList[animationIndex].isAuto)
        {
            ShowAutoCutScene(cutSceneCellList[animationIndex]);
        }
        else
        {
            ShowCutScene(cutSceneCellList[animationIndex]);
        }

        UIManager.Instance.UIShow = true;
        isPlayingCutScene = true;
    }

    /// <summary>
    /// 获取并排列好文本,并初始化文本与图片内容
    /// </summary>
    private void ShowCutScene(CutSceneCell cutSceneCell)
    {
        tmpText.text = Setting.stringDefaultValue;
        
        if (cutSceneCell.text != null)  
        {
            var rows = cutSceneCell.text.text.Split("\n");
            foreach (var row in rows)
            {
                textForShow.Enqueue(row);
            }
            StartCoroutine(PlayingRowText(textForShow.Dequeue()));
        }
        else
        {
            tmpText.text = Setting.stringDefaultValue;
        }

        if (cutSceneCell.music != null)
        {
            soundManager.Instance.StopMusicInFade();
            soundManager.Instance.PlayMusicInFade(cutSceneCell.music);
        }

        if (cutSceneCell.sfx != null)
        {
            soundManager.Instance.StopMusicInFade();
            soundManager.Instance.PlaySFX(cutSceneCell.sfx);
        }
        
        ChangeAnimation(cutSceneCell);
    }

    /// <summary>
    /// 启动文字逐一播放
    /// </summary>
    IEnumerator PlayingRowText(string rowText)
    {
        textFinished = false;

        var tag = rowText.Split(":");
        float R = float.Parse(tag[0]);
        float G = float.Parse(tag[1]);
        float B = float.Parse(tag[2]);
        float A = float.Parse(tag[3]);
        string textToPlay = tag[4];

        tmpText.color = new Color(R/255f, G/255f, B/255f, A/255f);
        tmpText.text = Setting.stringDefaultValue;
        int index = 0;
        while (!cancelTyping && index < textToPlay.Length-1)
        {
            soundManager.Instance.PlaySFX("Text");
            tmpText.text += textToPlay[index];
            index++;
            yield return new WaitForSeconds(playingTimeInterval);
        }

        tmpText.text = textToPlay;
        cancelTyping = false;
        textFinished = true;
    }
#endregion 

#region 自动播放动画字幕
    /// <summary>
    /// 展示自动播放场景
    /// </summary>
    /// <param name="cutSceneCell"></param>
    private void ShowAutoCutScene(CutSceneCell cutSceneCell)
    {
        tmpText.text = Setting.stringDefaultValue;

        if (cutSceneCell.text != null)  
        {
            var rows = cutSceneCell.text.text.Split("\n");
            foreach (var row in rows)
            {
                textForShow.Enqueue(row);
            }
            StartCoroutine(PlayingAutoText());
        }

        if (cutSceneCell.music != null)
        {
            soundManager.Instance.StopMusicInFade();
            soundManager.Instance.PlayMusicInFade(cutSceneCell.music);
        }

        if (cutSceneCell.sfx != null)
        {
            soundManager.Instance.StopMusicInFade();
            soundManager.Instance.PlaySFX(cutSceneCell.sfx);
        }

        ChangeAnimation(cutSceneCell);

        isPlayingAutoCutScene = true;
    }

    /// <summary>
    /// 自动播放文本内容
    /// </summary>
    private IEnumerator PlayingAutoText()
    {
        while (textForShow.Count != 0)
        {
            string rowText = textForShow.Dequeue();
            yield return StartCoroutine(PlayingAutoRowText(rowText));
        }
    }

    /// <summary>
    /// 启动自动文字逐一播放
    /// </summary>
    IEnumerator PlayingAutoRowText(string rowText)
    {
        var tag = rowText.Split(":");
        float R = float.Parse(tag[0]);
        float G = float.Parse(tag[1]);
        float B = float.Parse(tag[2]);
        float A = float.Parse(tag[3]);
        string textToPlay = tag[4];
        float time = float.Parse(tag[5]);

        textFinished = false;

        tmpText.color = new Color(R/255f, G/255f, B/255f, A/255f);
        tmpText.text = Setting.stringDefaultValue;
        int index = 0;
        while (index < textToPlay.Length-1)
        {
            soundManager.Instance.PlaySFX("Text");
            tmpText.text += textToPlay[index];
            index++;
            yield return new WaitForSeconds(playingTimeInterval);
        }
        tmpText.text = textToPlay;
        yield return new WaitForSeconds(time);

        textFinished = true;
    }
#endregion

    
}
