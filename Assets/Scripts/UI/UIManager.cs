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

    private Coroutine displayNodeTextForShowRoutine;
    public bool UIShow = false;

    public void CloseGraph()
    {
        graphNodeUI.gameObject.SetActive(false);
        UIShow = false;
    }

    public void DisplayNodeText(string nodeTextForShow)
    {
        if (!this.nodeTextForShow.gameObject.activeSelf)
        {
            this.nodeTextForShow.gameObject.SetActive(true);
        }

        this.nodeTextForShow.GetComponent<TMP_Text>().text = nodeTextForShow;
    }

    /// <summary>
    /// 启动展示文本协程
    /// </summary>
    public void StartDisplayNodeTextForShowRoutine(string nodeTextForShow)
    {
        if (displayNodeTextForShowRoutine != null)
        {
            StopCoroutine(displayNodeTextForShowRoutine);
        }

        displayNodeTextForShowRoutine = StartCoroutine(DisplayNodeTextForShow(nodeTextForShow));
    }

    /// <summary>
    /// 显示节点文本内容
    /// </summary>
    private IEnumerator DisplayNodeTextForShow(string nodeTextForShow)
    {
        this.nodeTextForShow.gameObject.SetActive(true);

        this.nodeTextForShow.GetComponent<TMP_Text>().text = nodeTextForShow;

        yield return new WaitForSeconds(2f);

        this.nodeTextForShow.gameObject.SetActive(false);
    }

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
}
