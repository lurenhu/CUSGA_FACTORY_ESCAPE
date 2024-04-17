using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEditor.EditorTools;

[DisallowMultipleComponent]
public class Node : MonoBehaviour
{
    [HideInInspector] public bool isPopping = false;// 判断是否处于弹出状态
    [HideInInspector] public bool isDragging = false;// 判断是否处于拖拽状态
    [HideInInspector] public bool isSelected = false;// 判断是否处于被选中状体
    [HideInInspector] public bool hasPopUp = false;// 判断节点是否已经弹出子节点

    [Header("观测参数")]
    public string id;
    public string parentID;
    public List<string> childIdList;
    public List<NodeInfo> nodeInfos;
    public List<AudioClip> audios;
    public string nodeTextForShow;
    [HideInInspector] public Rect rect;
    [HideInInspector] public NodeTypeSO nodeType;
    
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col2D;
    private Vector2 lastMouseWorldPosition;

    /// <summary>
    /// 初始化节点数据
    /// </summary>
    /// <param name="nodeInGraph"></param>
    public void InitializeNode(NodeSO nodeSO)
    {
        id = nodeSO.id;
        childIdList = CopyStringList(nodeSO.childrenNodeIdList);
        rect = nodeSO.rect;
        nodeType = nodeSO.nodeType;

        if (nodeSO.parentNodeIdList.Count == 0)
            parentID = Setting.stringDefaultValue;
        else
            parentID = nodeSO.parentNodeIdList[0];

        TMP_Text tMP_Text = transform.GetComponentInChildren<TMP_Text>();
        if (tMP_Text != null)
        {
            tMP_Text.text = nodeSO.nodeText;
        }

        audios = nodeSO.audioList;
        nodeTextForShow = nodeSO.nodeTextForShow;
    }

    protected virtual void Start()
    {
        LoadNodeInfo();

        MatchCollider2D();
    }

    /// <summary>
    /// 导入需要弹出的子节点集
    /// </summary>
    public void LoadNodeInfo()
    {
        if (childIdList == null)
        {
            return;
        }

        foreach (string childNodeID in childIdList)
        {
            Node childNode = NodeMapBuilder.Instance.GetNode(childNodeID);

            Vector2 direction = new Vector2((childNode.rect.center - rect.center).x, (rect.center - childNode.rect.center).y).normalized;

            NodeInfo newNodeInfo = new NodeInfo()
            {
                node = childNode,
                direction = direction
            };

            nodeInfos.Add(newNodeInfo);
        }
    }

    /// <summary>
    /// 将碰撞器的碰撞盒匹配图片
    /// </summary>
    private void MatchCollider2D()
    {
        if (nodeType == null)
        {
            nodeType = GameResources.Instance.nodeTypeList.list.Find(x => x.isDefault);
        }

        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        col2D = transform.GetComponent<BoxCollider2D>();

        if (col2D != null)
            col2D.size = spriteRenderer.sprite.bounds.size;
    }

    private void OnMouseDown() {
        lastMouseWorldPosition = HelperUtility.TranslateScreenToWorld(Input.mousePosition);
    }

    protected virtual void OnMouseDrag() 
    {
        if (isPopping || nodeType.isQTE || UIManager.Instance.UIShow) return;
        
        // 获取鼠标位移并将对象进行跟随鼠标进行位移
        Vector2 currentMouseWorldPosition = HelperUtility.TranslateScreenToWorld(Input.mousePosition);
        Vector2 mouseDelta = currentMouseWorldPosition - lastMouseWorldPosition;
        transform.Translate(mouseDelta);
        lastMouseWorldPosition = currentMouseWorldPosition;

        if (!isDragging && mouseDelta != Vector2.zero) 
        {
            GameManager.Instance.haveNodeDrag = true;
            isDragging = true;
        }
    }

    /// <summary>
    /// 弹出所有子节点
    /// </summary>
    public void PopUpChildNode(List<NodeInfo> nodes)
    {
        foreach (NodeInfo childNode in nodes)
        {
            Node currentNode = childNode.node;

            currentNode.transform.position = transform.position;
            currentNode.gameObject.SetActive(true);

            LineCreator.Instance.ShowLine(currentNode);

            currentNode.transform.DOMove(
                childNode.direction * GameManager.Instance.popUpForce,GameManager.Instance.tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentNode.isPopping = true;
                }).OnComplete(() => 
                {
                    currentNode.isPopping = false;
                });
        }
    }

    public void PopUpChildNode(NodeInfo node)
    {
        Node currentNode = node.node;

        currentNode.transform.position = transform.position;
        currentNode.gameObject.SetActive(true);

        LineCreator.Instance.ShowLine(currentNode);

        currentNode.transform.DOMove(
            node.direction * GameManager.Instance.popUpForce,GameManager.Instance.tweenDuring
            ).SetRelative().OnStart(() => 
            {
                currentNode.isPopping = true;
            }).OnComplete(() => 
            {
                currentNode.isPopping = false;
            });
    }

    

    /// <summary>
    /// 获取被选中动画
    /// </summary>
    public void GetSelectedAnimate()
    {
        if (spriteRenderer.material.color != Color.red)
        {
            spriteRenderer.material.DOColor(Color.red,1f);
        }
    }

    /// <summary>
    /// 获取不被选中动画
    /// </summary>
    public void GetUnSelectedAnimate()
    {
        if (spriteRenderer.material.color != Color.white)
        {
            spriteRenderer.material.DOColor(Color.white,1f);
        }
    }

    /// <summary>
    /// 复制字符串列表
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

}

[Serializable]
public class NodeInfo
{
    public Node node;// 将被弹出的节点
    public Vector2 direction;// 弹出方向偏移距离
}
