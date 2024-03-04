using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeHasCreated = new Dictionary<string, Node>();
    private Dictionary<string,NodeProperty> nodeToBuildDictionary = new Dictionary<string, NodeProperty>();
    private Dictionary<string,NodeTemplateSO> nodeTemplateDictionary = new Dictionary<string, NodeTemplateSO>();
    public List<NodeTemplateSO> nodeTemplateList;
    private NodeTypeListSO nodeTypeList;

    protected override void Awake()
    {
        base.Awake();

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    public bool GenerateNodeMap(NodeGraphSO nodeGraph)
    {
        bool nodeBuildSuccessful = false;
        int nodeBuildAttempts = 0;

        while (!nodeBuildSuccessful && nodeBuildAttempts < 10)
        {
            ClearNodes();

            nodeBuildAttempts++;

            nodeBuildSuccessful = AttempToBuildNodes(nodeGraph);
        }
        
        if (nodeBuildSuccessful)
        {
            InstantiateNodes();
        }

        return nodeBuildSuccessful;
    }

    private void InstantiateNodes()
    {
        foreach (KeyValuePair<string,NodeProperty> keyValue in nodeToBuildDictionary)
        {
            NodeProperty currenNode = keyValue.Value;

            GameObject nodeGameObject = Instantiate(currenNode.nodePrefab,transform.position,Quaternion.identity,transform);

            Node nodeCompounent = nodeGameObject.GetComponent<Node>();

            nodeCompounent.InitializeNode(currenNode);

            currenNode.node = nodeCompounent;

            nodeHasCreated.Add(nodeCompounent.id,nodeCompounent);
        }
    }

    private bool AttempToBuildNodes(NodeGraphSO nodeGraph)
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
            return false;
        }

        return ProcessNodeIntempNodeQueue(nodeGraph,tempNodeQueue);
    }

    private bool ProcessNodeIntempNodeQueue(NodeGraphSO nodeGraph, Queue<NodeSO> tempNodeQueue)
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

        return true;
    }

    private NodeProperty CreatNodeFromNodeTemplate(NodeSO currentNode, NodeTemplateSO nodeTemplate)
    {
        NodeProperty node = new NodeProperty
        {
            nodeText = currentNode.nodeText,
            id = currentNode.id,
            rect = currentNode.rect,
            nodePrefab = nodeTemplate.nodePrefab,
            childIdList = CopyStringList(currentNode.childrenNodeIdList)
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

    private NodeTemplateSO GetNodeTemplate(NodeTypeSO nodeType)
    {
        foreach (NodeTemplateSO nodeTemplate in nodeTemplateList)
        {
            if (nodeTemplate.nodeType == nodeType)
            {
                return nodeTemplate;
            }
        }

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

}
