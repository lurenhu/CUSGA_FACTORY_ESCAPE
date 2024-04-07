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
    public Transform GraphNodeUI;
    [Tooltip("将节点文本显示的UI对象")]
    public Transform nodeTextForShow;
    [Tooltip("滑动文本框内的文本")]
    public Transform scrollViewContent;
    [Tooltip("节点文本显示UI对象")]
    public Transform TextNodeUI;

    [HideInInspector] public bool UIShow = false;

    public void CloseGraph()
    {
        GraphNodeUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示节点文本内容
    /// </summary>
    public IEnumerator DisplayNodeTextForShow(string nodeTextForShow)
    {
        this.nodeTextForShow.gameObject.SetActive(true);

        this.nodeTextForShow.GetComponent<TMP_Text>().text = nodeTextForShow;

        yield return new WaitForSeconds(2f);

        this.nodeTextForShow.gameObject.SetActive(false);
    }

    public void CloseTextNodeUI()
    {
        TextNodeUI.gameObject.SetActive(false);
    } 

    /// <summary>
    /// 展示文本节点UI
    /// </summary>
    public void DisplayTextNodeContent(string text) 
    {
        scrollViewContent.GetComponent<TMP_Text>().text = text;

        TextNodeUI.gameObject.SetActive(true);
    }
}
