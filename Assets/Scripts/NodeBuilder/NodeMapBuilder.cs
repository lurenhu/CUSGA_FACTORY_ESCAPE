using System.Collections.Generic;
using UnityEngine;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeToBuildDictionary = new Dictionary<string, Node>();
    public Queue<Node> nodeToBuildQueue = new Queue<Node>();
    public List<NodeTemplateSO> nodeTemplateList = null;

    protected override void Awake()
    {
        base.Awake();
        nodeTemplateList = GameResources.Instance.nodeTemplateList;
    }

    public bool GenerateNodeMap(NodeGraphSO nodeGraph)
    {
        LoadNodeToBuildDictionary(nodeGraph);

        nodeToBuildDictionary.TryGetValue(nodeGraph.nodeList[0].id,out Node randomNode);

        Node rootNode = FindRootNode(randomNode);

        LoadNodeTreeToQueue(rootNode);

        InstantiateNode();
        
        return false;
    }

    private void LoadNodeToBuildDictionary(NodeGraphSO nodeGraph)
    {
        foreach (NodeSO node in nodeGraph.nodeList)
        {
            foreach (NodeTemplateSO nodeTemplate in nodeTemplateList)
            {
                if (node.nodeType == nodeTemplate.nodeType)
                {
                    Node nodeToBuild = nodeTemplate.nodePrefab.GetComponent<Node>();
                    nodeToBuild.InitializeNode(node);
                    nodeToBuildDictionary.Add(node.id,nodeToBuild);
                }
            }
        }
    }

    /// <summary>
    /// 找到所有节点的根节点
    /// </summary>
    private Node FindRootNode(Node node)
    {
        Node rootNode = node;
        while (CheckCurrentNodeHasParent(rootNode))
        {
            if (nodeToBuildDictionary.TryGetValue(rootNode.parentNodeID,out rootNode))
            {
                Debug.Log("Find parent Node " + rootNode.id);
            }
            else
            {
                Debug.Log("Node has not parent Node " + rootNode.id);
            }
        }

        return rootNode;
    }

    /// <summary>
    /// 判断当前节点是否有父节点
    /// </summary>
    private bool CheckCurrentNodeHasParent(Node node)
    {
        if (node.parentNodeID != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断当前节点是否有子节点
    /// </summary>
    private bool CheckCurrentNodeHasChild(Node node)
    {
        if (node.childNodeIDList.Count > 0)
        {
            return true;
        }
        return false;
    }

    private void InstantiateNode()
    {
        Node rootNode = nodeToBuildQueue.Dequeue();

        Instantiate(rootNode,transform.position,Quaternion.identity,transform);
        for (int i = 0; i < nodeToBuildQueue.Count; i++)
        {
            Node nodeToBild = nodeToBuildQueue.Dequeue();
            Node currentNode = Instantiate(nodeToBild,transform.position,Quaternion.identity,transform);

            currentNode.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 将节点树层序遍历
    /// </summary>
    private void LoadNodeTreeToQueue(Node rootNode)
    {
        Queue<Node> tempQueue = new Queue<Node>();

        if (rootNode != null)
        {
            tempQueue.Enqueue(rootNode);
        }

        while (tempQueue.Count > 0)
        {
            foreach (Node currentNode in tempQueue)
            {
                if (CheckCurrentNodeHasChild(currentNode))
                {
                    foreach (string childNodeID in currentNode.childNodeIDList)
                    {
                        nodeToBuildDictionary.TryGetValue(childNodeID,out Node node);
                        tempQueue.Enqueue(node);
                    }
                    nodeToBuildQueue.Enqueue(tempQueue.Dequeue());
                }
            }
        }
    }

}
