using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    [Space(10)]
    [Header("所需UI对象")]
    [Tooltip("图节点展示图的UI对象")]
    public Transform graphNodeUI;
    [Tooltip("将节点文本显示的UI对象")]
    public Transform nodeTextForShow;
    [Tooltip("滑动文本框内的文本")]
    public Transform scrollViewContent;
    [Tooltip("节点文本显示UI对象")]
    public Transform textNodeUI;
    [Tooltip("背景UI对象")]
    public Transform backGround;
    [Tooltip("向右侧切换节点图按钮")]
    public Transform rightNodeGraphButton;
    [Tooltip("向左侧切换节点图按钮")]
    public Transform leftNodeGraphButton;
    [Tooltip("AI对话日志")]
    public Transform AIDialogLog;
    

    public bool UIShow = false;

    /// <summary>
    /// 关闭图片节点UI
    /// </summary>
    public void CloseGraph()
    {
        graphNodeUI.gameObject.SetActive(false);
        UIShow = false;
    }

    /// <summary>
    /// 展示该节点的文本内容
    /// </summary>
    public void DisplayNodeText(string nodeTextForShow)
    {
        if (!this.nodeTextForShow.gameObject.activeSelf)
        {
            this.nodeTextForShow.gameObject.SetActive(true);
        }

        this.nodeTextForShow.GetComponent<TMP_Text>().text = nodeTextForShow;
    }

    /// <summary>
    /// 关闭文本节点UI
    /// </summary>
    public void CloseTextNodeUI()
    {
        textNodeUI.gameObject.SetActive(false);
        UIShow = false;
    } 

    /// <summary>
    /// 展示文本节点UI
    /// </summary>
    public void DisplayTextNodeContent(TextAsset text) 
    {
        scrollViewContent.GetComponent<TMP_Text>().text = text.text;

        textNodeUI.gameObject.SetActive(true);
        UIShow = true;
    }

    /// <summary>
    /// 打开与关闭AI日志
    /// </summary>
    public void DisplayAndCloseAILog()
    {
        AIDialogLog.gameObject.SetActive(!AIDialogLog.gameObject.activeSelf);
    }

    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSecounds, Color color)
    {
        Image image = backGround.GetComponent<Image>();
        image.color = color;

        float time = 0;

        while (time <= fadeSecounds)
        {
            time += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time/fadeSecounds));
            yield return null;
        }

    }
}
