using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
public class Node : MonoBehaviour
{
    [HideInInspector] public bool isPopping = false;// 判断是否处于弹出状态
    [HideInInspector] public bool isDragging = false;// 判断是否处于拖拽状态
    [HideInInspector] public bool isSelected = false;// 判断是否处于被选中状体
    [HideInInspector] public bool hasPopUp = false;// 判断节点是否已经弹出子节点

    [Header("观测参数")]
    public string id;
    public string nodeText;
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
        nodeText = nodeSO.nodeText;
        childIdList = nodeSO.childrenNodeIdList;
        rect = nodeSO.rect;
        nodeType = nodeSO.nodeType;

        if (nodeSO.parentNodeIdList.Count == 0)
            parentID = Setting.stringDefaultValue;
        else
            parentID = nodeSO.parentNodeIdList[0];
    }

    protected virtual void Start()
    {
        LoadNodeInfo();
    }
    
    private void LoadNodeInfo()
    {
        if (childIdList == null)
        {
            return;
        }

        foreach (string childNodeID in childIdList)
        {
            NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(childNodeID,out Node childNode);

            Vector2 direction = (rect.center - childNode.rect.center).normalized;

            NodeInfo newNodeInfo = new NodeInfo()
            {
                node = childNode,
                direction = direction
            };

            nodeInfos.Add(newNodeInfo);
        }
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

            //currentNode.nodeProperty.nodeTextInstance.gameObject.SetActive(true);

            // LineCreator.Instance.CreateLine(currentNode);

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
        
        // currentNode.nodeProperty.nodeTextInstance.gameObject.SetActive(true);

        // LineCreator.Instance.CreateLine(currentNode);

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

    

}

[Serializable]
public class NodeInfo
{
    public Node node;
    public Vector2 direction;// 弹出方向偏移距离
}

[Serializable]
public class NodeProperty
{
    public string id;
    public string nodeText;
    public string parentID;
    public List<string> childIdList;
    [HideInInspector] public Rect rect;
    [HideInInspector] public GameObject nodePrefab;
    [HideInInspector] public Node node;
    [HideInInspector] public NodeTypeSO nodeType;


    [Space(10)]
    [Header("合成节点数据")]
    public string targetNodeID;

    [Space(10)]
    [Header("锁节点数据")]
    public List<int> cipherValues;

    [Space(10)]
    [Header("图片节点数据")]
    public Sprite image;

    [Space(10)]
    [Header("角度锁节点数据")]
    public List<float> angles;

    [Space(10)]
    [Header("探测节点数据")]  
    public string targetIDForDetection;
}
