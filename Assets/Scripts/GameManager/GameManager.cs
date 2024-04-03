using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SingletonMonobehaviour<GameManager>
{

    [Space(10)]
    [Header("动画参数")]
    [Tooltip("弹出动画的持续时间")]
    public float tweenDuring = 0.5f;// 弹出持续时间
    [Tooltip("弹出动画的弹出距离")]
    public float popUpForce = 3;// 弹出距离

    [Space(10)]
    [Header("节点图参数")]
    [Tooltip("所需生成节点图列表")]
    public List<NodeGraphSO> nodeGraphSOs;

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
    [HideInInspector] public bool haveNodeDrag = false;

    override protected void Awake() {
        base.Awake();
    }
    
    private void Start() {
        NodeMapBuilder.Instance.GenerateNodeMap(nodeGraphSOs[0]);
    }

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
