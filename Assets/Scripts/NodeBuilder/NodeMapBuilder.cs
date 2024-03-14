using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeHasCreated = new Dictionary<string, Node>();
    private Queue<NodeData> nodeProperties = new Queue<NodeData>();
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
            NodeData currentNode = nodeProperties.Dequeue();

            GameObject nodeGameObject = Instantiate(currentNode.nodePrefab,transform.position,Quaternion.identity,transform);

            if (!currentNode.nodeSO.nodeType.isEntrance)
            {
                nodeGameObject.SetActive(false);
            }

            Node nodeComponent = nodeGameObject.GetComponent<Node>();
            
            nodeComponent.InitializeNode(currentNode.nodeSO);

            MatchCorrespondingNodeType(currentNode.nodeSO, nodeGameObject);

            nodeHasCreated.Add(nodeComponent.id,nodeComponent);
        }
    }

    /// <summary>
    /// 匹配对应节点类型的组件
    /// </summary>
    private void MatchCorrespondingNodeType(NodeSO currentNode, GameObject nodeGameObject)
    {
        if (currentNode.nodeType.isDefault || currentNode.nodeType.isEntrance)
        {
        }
        else if (currentNode.nodeType.isAI)
        {
        }
        else if (currentNode.nodeType.isLocked)
        {
            CipherLocked cipherLocked = nodeGameObject.GetComponent<CipherLocked>();
            cipherLocked.InitializeCipherLocked(currentNode);
        }
        else if (currentNode.nodeType.isAngleLock)
        {
            AngleLocked angleLocked = nodeGameObject.GetComponent<AngleLocked>();
            angleLocked.InitializeAngleLocked(currentNode);
        }
        else if (currentNode.nodeType.isGraph)
        {
            Graph graph = nodeGameObject.GetComponent<Graph>();
            graph.InitializeGraph(currentNode);
        }
        else if (currentNode.nodeType.isControllable)
        { 

        }
        else if (currentNode.nodeType.isProbe)
        {
            Probe probe = nodeGameObject.GetComponent<Probe>();   
            probe.InitializeProbe(currentNode);
        }
        else if (currentNode.nodeType.isSynthetic)
        {
            Synthesizer synthesizer = nodeGameObject.GetComponent<Synthesizer>();
            synthesizer.InitializeSynthesizer(currentNode);
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
        // 加入独立的节点
        foreach (NodeSO nodeSO in nodeGraph.nodeList)
        {
            if (nodeSO.parentNodeIdList.Count == 0 && !nodeSO.nodeType.isEntrance)
            {
                NodeTemplateSO nodeTemplate = GetNodeTemplate(nodeSO.nodeType);

                NodeData nodeData = CreateNodeFromNodeTemplate(nodeSO,nodeTemplate);

                nodeProperties.Enqueue(nodeData);
            }
        }

        // 加入整个节点树中的节点
        while (tempNodeQueue.Count > 0)
        {
            NodeSO currentNode = tempNodeQueue.Dequeue();

            foreach (NodeSO childNode in nodeGraph.GetChildNodes(currentNode))
            {
                tempNodeQueue.Enqueue(childNode);
            }

            NodeTemplateSO nodeTemplate = GetNodeTemplate(currentNode.nodeType);

            NodeData nodeData = CreateNodeFromNodeTemplate(currentNode,nodeTemplate);

            nodeProperties.Enqueue(nodeData);
        }
    }

    /// <summary>
    /// 通过节点模板类来将节点属性初始化
    /// </summary>
    private NodeData CreateNodeFromNodeTemplate(NodeSO currentNode, NodeTemplateSO nodeTemplate)
    {
        NodeData node = new NodeData();
        
        // 节点的基本属性赋值
        node.nodeSO = currentNode;
        node.nodePrefab = nodeTemplate.nodePrefab;

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
