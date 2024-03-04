using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class Node : MonoBehaviour
{
    [Space(10)]
    [Header("NODE CLASS PROPERTISE")]
    
    [Tooltip("弹出动画的持续时间")]
    [SerializeField] protected float tweenDuring = 2;// 弹出持续时间
    [SerializeField] protected float popUpForce = 2;
    [HideInInspector] public bool isPoping = false;// 判断是否处于弹出状态
    [HideInInspector] public bool isDraging = false;// 判断是否处于拖拽状态
    protected bool isSelected = false;// 判断是否处于被选中状体
    protected bool hasPopUp = false;// 判断节点是否已经弹出子节点

    public string id;
    [Tooltip("该节点的所有子节点")]
    public List<string> childNodeIDList;// 存储子节点集
    [Tooltip("该节点的父节点")]
    public string parentNodeID;// 当前节点的父节点
    public Rect rect;
    public string nodeText;
    public List<NodeInfo> nodeInfos;

    /// <summary>
    /// 初始化节点数据
    /// </summary>
    /// <param name="nodeInGraph"></param>
    public void InitializeNode(NodeProperty nodeInGraph)
    {
        this.id = nodeInGraph.id;
        this.childNodeIDList = nodeInGraph.childIdList;
        this.parentNodeID = nodeInGraph.parentID;
        this.rect = nodeInGraph.rect;
        this.nodeText = nodeInGraph.nodeText;
    }

    protected virtual void Start()
    {
        LoadNodeInfo();
    }
    
    private void LoadNodeInfo()
    {
        if (childNodeIDList == null)
        {
            return;
        }

        foreach (string childNodeID in childNodeIDList)
        {
            NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(childNodeID,out Node childNode);

            Vector2 direction = (childNode.rect.center - rect.center).normalized;

            NodeInfo newNodeInfo = new NodeInfo()
            {
                node = childNode,
                direction = direction
            };

            nodeInfos.Add(newNodeInfo);
        }
    }

    protected virtual void OnMouseUp()
    {
        if (isDraging) isDraging = false;

        if (isPoping) return;
    }

    protected virtual void OnMouseDrag() {
        if (!isDraging)
            isDraging = true;
            
        if (!isPoping)
            transform.position = TranslateScreenToWorld(Input.mousePosition);
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

            NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(currentNode.parentNodeID,out Node parentNode);
            currentNode.transform.GetComponentInChildren<Line>().endPoint = parentNode.transform;

            currentNode.transform.DOMove(
                childNode.direction * popUpForce,tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentNode.isPoping = true;
                }).OnComplete(() => 
                {
                    currentNode.isPoping = false;
                });
        }
    }

    /// <summary>
    /// 将屏幕坐标转化为世界坐标
    /// </summary>
    public static Vector3 TranslateScreenToWorld(Vector3 Position)
    {
        Vector3 cameraTranslatePos = Camera.main.ScreenToWorldPoint(Position);
        return new Vector3 (cameraTranslatePos.x,cameraTranslatePos.y,0);
    }

}

[Serializable]
public class NodeInfo
{
    public Node node;
    public Vector2 direction;// 弹出方向偏移距离

}

public class NodeProperty
{
    public Rect rect;
    public string id;
    public string nodeText;
    public string parentID;
    public List<string> childIdList;
    public GameObject nodePrefab;
    public Node node;
}
