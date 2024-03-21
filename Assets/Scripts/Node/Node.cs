using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using TMPro;

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
    [HideInInspector] public Rect rect;
    [HideInInspector] public GameObject nodePrefab;
    [HideInInspector] public Node node;
    [HideInInspector] public NodeTypeSO nodeType;

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
    }

    protected virtual void Start()
    {
        LoadNodeInfo();
    }
    
    protected virtual void OnMouseDrag() 
    {
        if (isPopping) return;

        if (!isDragging) isDragging = true;
        
        transform.position = HelperUtility.TranslateScreenToWorld(Input.mousePosition);
    }

    /// <summary>
    /// 弹出所有子节点
    /// </summary>
    public void PopUpChildNode(List<NodeInfo> nodes)
    {
        foreach (NodeInfo childNode in nodes)
        {
            Node currentNode = childNode.node; // Instantiate(childNode.node,transform.position,Quaternion.identity);

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
        Node currentNode = node.node; // Instantiate(childNode.node,transform.position,Quaternion.identity);

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
    /// 导入需要弹出的子节点集
    /// </summary>
    private void LoadNodeInfo()
    {
        if (childIdList == null)
        {
            return;
        }

        foreach (string childNodeID in childIdList)
        {
            NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(childNodeID,out Node childNode);

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
    public Node node;
    public Vector2 direction;// 弹出方向偏移距离
}
