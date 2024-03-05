using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeHasCreated = new Dictionary<string, Node>();
    private Dictionary<string,NodeProperty> nodeToBuildDictionary = new Dictionary<string, NodeProperty>();
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
        foreach (KeyValuePair<string,NodeProperty> keyValue in nodeToBuildDictionary)
        {
            NodeProperty currentNode = keyValue.Value;

            GameObject nodeGameObject = Instantiate(currentNode.nodePrefab,transform.position,Quaternion.identity,transform);

            if (!currentNode.nodeType.isEntrence)
            {
                nodeGameObject.SetActive(false);
            }

            Node nodeCompounent = nodeGameObject.GetComponent<Node>();

            nodeCompounent.InitializeNode(currentNode);

            currentNode.node = nodeCompounent;

            nodeHasCreated.Add(nodeCompounent.id,nodeCompounent);
        }
    }

    /// <summary>
    /// 尝试创建节点
    /// </summary>
    /// <param name="nodeGraph"></param>
    private void AttemptToBuildNodes(NodeGraphSO nodeGraph)
    {
        Queue<NodeSO> tempNodeQueue = new Queue<NodeSO>();

        NodeSO Entrence = nodeGraph.GetNode(nodeTypeList.list.Find(x => x.isEntrence));

        if (Entrence != null)
        {
            tempNodeQueue.Enqueue(Entrence);
        }
        else
        {
            Debug.Log("No entrance Node");
            return;
        }

        ProcessNodeIntempNodeQueue(nodeGraph, tempNodeQueue);
    }

    /// <summary>
    /// 通过队列来层序遍历此节点树
    /// </summary>
    private void ProcessNodeIntempNodeQueue(NodeGraphSO nodeGraph, Queue<NodeSO> tempNodeQueue)
    {
        while (tempNodeQueue.Count > 0)
        {
            NodeSO currentNode = tempNodeQueue.Dequeue();

            foreach (NodeSO childNode in nodeGraph.GetChildNodes(currentNode))
            {
                tempNodeQueue.Enqueue(childNode);
            }

            NodeTemplateSO nodeTemplate = GetNodeTemplate(currentNode.nodeType);

            NodeProperty node = CreatNodeFromNodeTemplate(currentNode,nodeTemplate);

            nodeToBuildDictionary.Add(node.id,node);
        }
    }

    /// <summary>
    /// 通过节点模板类来将节点属性初始化
    /// </summary>
    private NodeProperty CreatNodeFromNodeTemplate(NodeSO currentNode, NodeTemplateSO nodeTemplate)
    {
        NodeProperty node = new NodeProperty
        {
            nodeText = currentNode.nodeText,
            id = currentNode.id,
            rect = currentNode.rect,
            nodePrefab = nodeTemplate.nodePrefab,
            childIdList = CopyStringList(currentNode.childrenNodeIdList),
            nodeType = currentNode.nodeType,

            targetNodeID = currentNode.tragetId,

            cipherNumber = currentNode.cipherNumber,
            cipherValues = currentNode.cipherValues
        };

        if (currentNode.parentNodeIdList.Count == 0)
        {
            node.parentID = "";
        }
        else    
        {
            node.parentID = currentNode.parentNodeIdList[0];
        }

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

    /// <summary>
    /// 清理所有建立好的节点及其字典
    /// </summary>
    private void ClearNodes()
    {
        if (nodeToBuildDictionary.Count > 0)
        {
            foreach (KeyValuePair<string,NodeProperty> keyValuePair in nodeToBuildDictionary)
            {
                NodeProperty node = keyValuePair.Value;

                if (node.node != null)
                {
                    Destroy(node.node.gameObject);
                }
            }
        }

        nodeToBuildDictionary.Clear();
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

}
