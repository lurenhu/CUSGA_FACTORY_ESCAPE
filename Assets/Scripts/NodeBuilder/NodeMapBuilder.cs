using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeHasCreated = new Dictionary<string, Node>();
    private Queue<NodeProperty> nodeProperties = new Queue<NodeProperty>();
    public List<NodeTemplateSO> nodeTemplateList;
    private NodeTypeListSO nodeTypeList;

    protected override void Awake()
    {
        base.Awake();

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    public bool GenerateNodeMap(NodeGraphSO nodeGraph)
    {
        AttemptToBuildNodes(nodeGraph);
        
        InstantiateNodes();

        return true;
    }

    /// <summary>
    /// 将所有节点实例化
    /// </summary>
    private void InstantiateNodes()
    {
        while (nodeProperties.Count > 0)
        {
            NodeProperty currentNode = nodeProperties.Dequeue();

            GameObject nodeGameObject = Instantiate(currentNode.nodePrefab,transform.position,Quaternion.identity,transform);

            if (!currentNode.nodeType.isEntrance)
            {
                nodeGameObject.SetActive(false);
            }

            Node nodeComponent = nodeGameObject.GetComponent<Node>();

            nodeComponent.InitializeNode(currentNode);

            currentNode.node = nodeComponent;

            nodeHasCreated.Add(nodeComponent.id,nodeComponent);
        }
    }

    /// <summary>
    /// 尝试创建节点
    /// </summary>
    /// <param name="nodeGraph"></param>
    private void AttemptToBuildNodes(NodeGraphSO nodeGraph)
    {
        Queue<NodeSO> tempNodeQueue = new Queue<NodeSO>();

        NodeSO Entrance = nodeGraph.GetNode(nodeTypeList.list.Find(x => x.isEntrance));

        if (Entrance != null)
        {
            tempNodeQueue.Enqueue(Entrance);
        }
        else
        {
            Debug.Log("No entrance Node");
            return;
        }

        ProcessNodeInTempNodeQueue(nodeGraph, tempNodeQueue);
    }

    /// <summary>
    /// 通过队列来层序遍历此节点树
    /// </summary>
    private void ProcessNodeInTempNodeQueue(NodeGraphSO nodeGraph, Queue<NodeSO> tempNodeQueue)
    {
        while (tempNodeQueue.Count > 0)
        {
            NodeSO currentNode = tempNodeQueue.Dequeue();

            foreach (NodeSO childNode in nodeGraph.GetChildNodes(currentNode))
            {
                tempNodeQueue.Enqueue(childNode);
            }

            NodeTemplateSO nodeTemplate = GetNodeTemplate(currentNode.nodeType);

            NodeProperty node = CreateNodeFromNodeTemplate(currentNode,nodeTemplate);

            nodeProperties.Enqueue(node);
        }
    }

    /// <summary>
    /// 通过节点模板类来将节点属性初始化
    /// </summary>
    private NodeProperty CreateNodeFromNodeTemplate(NodeSO currentNode, NodeTemplateSO nodeTemplate)
    {
        NodeProperty node = new NodeProperty();
        
        // 节点的基本属性赋值
        node.nodeText = currentNode.nodeText;
        node.id = currentNode.id;
        node.rect = currentNode.rect;
        node.nodePrefab = nodeTemplate.nodePrefab;
        node.childIdList = CopyStringList(currentNode.childrenNodeIdList);
        node.nodeType = currentNode.nodeType;
        if (currentNode.parentNodeIdList.Count == 0)
        {
            node.parentID = Setting.stringDefaultValue;
        }
        else    
        {
            node.parentID = currentNode.parentNodeIdList[0];
        }

        // 对应节点类型属性赋值
        if (currentNode.nodeType.isSynthetic)
            node.targetNodeID = (currentNode as SynthesizableNodeSO).targetIdForMerge;// 合成节点-合成目标
        else if (currentNode.nodeType.isLocked)
            node.cipherValues = (currentNode as LockedNodeSO).cipherValues;// 密码锁节点-密码值列表
        else if (currentNode.nodeType.isGraph)
            node.image = (currentNode as GraphNodeSO).image;// 图节点-图片
        else if (currentNode.nodeType.isAngleLock)
            node.angles = (currentNode as AngleLockNodeSO).angles;// 角度锁节点-角度值列表
        else if (currentNode.nodeType.isProbe)
            node.targetIDForDetection = (currentNode as ProbeNodeSO).targetIDForDetection;// 探测节点-探测目标

        return node;
    }

    /// <summary>
    /// 获取节点模板类
    /// </summary>
    private NodeTemplateSO GetNodeTemplate(NodeTypeSO nodeType)
    {
        foreach (NodeTemplateSO nodeTemplate in nodeTemplateList)
        {
            if (nodeTemplate.nodeType == nodeType)
            {
                return nodeTemplate;
            }
        }
        Debug.Log("Not find the nodeTemplate");
        return null;
    }

    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

    public void ClearAllSelectedNode(Node node)
    {
        foreach (KeyValuePair<string,Node> keyValuePair in nodeHasCreated)
        {
            Node currentNode = keyValuePair.Value;

            if (currentNode != node && currentNode.isSelected)
            {
                currentNode.isSelected = false;
            }
        }
    }

    /// <summary>
    /// 通过节点ID获取场景中的节点
    /// </summary>
    public Node GetNode(string nodeID)
    {
        nodeHasCreated.TryGetValue(nodeID,out Node node);
        return node;
    }

}
