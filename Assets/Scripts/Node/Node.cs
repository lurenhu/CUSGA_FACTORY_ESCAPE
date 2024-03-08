using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEditor.EditorTools;

public class Node : MonoBehaviour
{
    [Space(10)]
    [Header("NODE CLASS PROPERTISE")]
    
    [Tooltip("弹出动画的持续时间")]
    [SerializeField] protected float tweenDuring = 0.5f;// 弹出持续时间
    [Tooltip("弹出动画的弹出距离")]
    [SerializeField] protected float popUpForce = 3;// 弹出距离
    [HideInInspector] public bool isPopping = false;// 判断是否处于弹出状态
    [HideInInspector] public bool isDragging = false;// 判断是否处于拖拽状态
    public bool isSelected = false;// 判断是否处于被选中状体
    [HideInInspector] public bool hasPopUp = false;// 判断节点是否已经弹出子节点

    [Tooltip("节点ID")]
    public string id;
    [Tooltip("节点所有属性")]
    public NodeProperty nodeProperty;
    [Tooltip("生成节点及节点生成方向")]
    public List<NodeInfo> nodeInfos;

    /// <summary>
    /// 初始化节点数据
    /// </summary>
    /// <param name="nodeInGraph"></param>
    public void InitializeNode(NodeProperty nodeInGraph)
    {
        this.id = nodeInGraph.id;
        this.nodeProperty = nodeInGraph;
    }

    protected virtual void Start()
    {
        LoadNodeInfo();
    }
    
    private void LoadNodeInfo()
    {
        if (nodeProperty.childIdList == null)
        {
            return;
        }

        foreach (string childNodeID in nodeProperty.childIdList)
        {
            NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(childNodeID,out Node childNode);

            Vector2 direction = (childNode.nodeProperty.rect.center - nodeProperty.rect.center).normalized;

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
        if (isPopping) return;

        if (isDragging) isDragging = false;

        // 节点交互事件--提示文字，音频
    }

    protected virtual void OnMouseDrag() 
    {
        if (isPopping) return;

        if (!isDragging) isDragging = true;
            
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

            LineCreator.Instance.CreateLine(currentNode);

            currentNode.transform.DOMove(
                childNode.direction * popUpForce,tweenDuring
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

        LineCreator.Instance.CreateLine(currentNode);

        currentNode.transform.DOMove(
            node.direction * popUpForce,tweenDuring
            ).SetRelative().OnStart(() => 
            {
                currentNode.isPopping = true;
            }).OnComplete(() => 
            {
                currentNode.isPopping = false;
            });
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

[Serializable]
public class NodeProperty
{
    [Header("普通节点数据")]
    [HideInInspector] public Rect rect;
    [HideInInspector] public string id;
    public string nodeText;
    public string parentID;
    public List<string> childIdList;
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
