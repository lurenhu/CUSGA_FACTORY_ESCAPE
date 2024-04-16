using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeMapBuilder : SingletonMonobehaviour<NodeMapBuilder>
{
    public Dictionary<string,Node> nodeHasCreated = new Dictionary<string, Node>();
    public List<NodeTemplateSO> nodeTemplateList;
    private Queue<NodeData> nodeProperties = new Queue<NodeData>();
    private NodeTypeListSO nodeTypeList;

    protected override void Awake()
    {
        base.Awake();

        nodeTypeList = GameResources.Instance.nodeTypeList;
    }

    /// <summary>
    /// 生成节点图
    /// </summary>
    public void GenerateNodeMap(NodeGraphSO nodeGraph)
    {
        InitEnv(nodeGraph);

        AttemptToBuildNodes(nodeGraph);
        
        InstantiateNodes();

        LocateCameraAtEntranceNode();
    }

    /// <summary>
    /// 规定节点图生成前的环境
    /// </summary>
    private void InitEnv(NodeGraphSO nodeGraph)
    {
        nodeHasCreated.Clear();
        nodeProperties.Clear();
        LineCreator.Instance.nodeLineBinding.Clear();

        if (nodeGraph.backGround != null)
        {
            UIManager.Instance.backGround.GetComponent<Image>().sprite = nodeGraph.backGround;
        }

    }

    /// <summary>
    /// 将相机对准入口节点
    /// </summary>
    private void LocateCameraAtEntranceNode()
    {
        foreach (KeyValuePair<string,Node> keyValuePair in nodeHasCreated)
        {
            Node currentNode = keyValuePair.Value;
            
            if (currentNode.nodeType.isEntrance)
            {
                Camera.main.transform.position = currentNode.transform.position + new Vector3(0, 0, -10);
                return;
            }
            else
            {
                Debug.Log("No Entrance to Locate");
            }
        }
    }

    /// <summary>
    /// 将所有节点实例化
    /// </summary>
    private void InstantiateNodes()
    {
        while (nodeProperties.Count > 0)
        {
            NodeData currentNode = nodeProperties.Dequeue();

            Vector2 offset = HelperUtility.TranslateScreenToWorld(currentNode.nodeSO.rect.center);
            Vector2 spawnPosition = new Vector2(offset.x, -offset.y);

            GameObject nodeGameObject = Instantiate(currentNode.nodePrefab,spawnPosition,Quaternion.identity,transform);

            Node nodeComponent = nodeGameObject.GetComponent<Node>();

            nodeComponent.InitializeNode(currentNode.nodeSO);

            MatchCorrespondingNodeType(currentNode.nodeSO, nodeGameObject);

            CreateLines(nodeComponent);

            if (!currentNode.nodeSO.nodeType.isEntrance)
            {
                nodeGameObject.SetActive(false);
            }

            nodeHasCreated.Add(nodeComponent.id,nodeComponent);
        }
    }

    /// <summary>
    /// 实例化线条对象
    /// </summary>
    public void CreateLines(Node node)
    {
        if (node.parentID != Setting.stringDefaultValue)
        {
            LineCreator.Instance.CreateLine(node);
        }
        else 
        {
            Debug.Log(node.name + " Not Parent");
        }
    }

    /// <summary>
    /// 匹配对应节点类型的组件并进行对应的初始化
    /// </summary>
    private void MatchCorrespondingNodeType(NodeSO currentNode, GameObject nodeGameObject)
    {
        if (currentNode.nodeType.isDefault || currentNode.nodeType.isEntrance || currentNode.nodeType.isExit)// 不需要进行初始化的节点类型
        {
            return;
        }
        else if (currentNode.nodeType.isAI)
        {
            AILocked aiLocked = nodeGameObject.GetComponent<AILocked>();
            aiLocked.InitializeAILocked(currentNode);
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
            Controllable controllable = nodeGameObject.GetComponent<Controllable>();
            controllable.InitializeControllable(currentNode);
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
        else if (currentNode.nodeType.isSyntheticPicture)
        {
            SyntheticPicture syntheticPicture = nodeGameObject.GetComponent<SyntheticPicture>();
            syntheticPicture.InitializeSyntheticPicture(currentNode);
        }
        else if (currentNode.nodeType.isTiming)
        {
            Timing timing = nodeGameObject.GetComponent<Timing>();
            timing.InitializeTiming(currentNode);
        }
        else if (currentNode.nodeType.isQuickClick)
        {
            QuickClick quickClick = nodeGameObject.GetComponent<QuickClick>();
            quickClick.InitializeQuickClick(currentNode);
        }
        else if (currentNode.nodeType.isQTE)
        {
            QTE qTE = nodeGameObject.GetComponent<QTE>();
            qTE.InitializeQTE(currentNode);
        }
        else if (currentNode.nodeType.isDialog)
        {
            Dialog dialog = nodeGameObject.GetComponent<Dialog>();
            dialog.InitializeDialog(currentNode);
        }
        else if (currentNode.nodeType.isText)
        {
            TextShow textShow = nodeGameObject.GetComponent<TextShow>();
            textShow.InitializeTextNode(currentNode);
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

        // 加入独立的节点
        foreach (NodeSO nodeSO in nodeGraph.nodeList)
        {
            if (nodeSO.parentNodeIdList.Count == 0 && !nodeSO.nodeType.isEntrance)
            {
                if (nodeSO.childrenNodeIdList.Count > 0)
                {
                    tempNodeQueue.Enqueue(nodeSO);
                }
                else
                {
                    NodeTemplateSO nodeTemplate = GetNodeTemplate(nodeSO.nodeType);

                    NodeData nodeData = CreateNodeFromNodeTemplate(nodeSO,nodeTemplate);

                    nodeProperties.Enqueue(nodeData);
                }
            }
        }

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

    /// <summary>
    /// 清除所有选中的节点
    /// </summary>
    public void ClearAllSelectedNode(Node node)
    {
        foreach (KeyValuePair<string,Node> keyValuePair in nodeHasCreated)
        {
            Node currentNode = keyValuePair.Value;

            if (currentNode != node && currentNode.isSelected)
            {
                currentNode.isSelected = false;
                currentNode.GetUnSelectedAnimate();
            }
        }
    }

    /// <summary>
    /// 通过节点ID获取场景中的节点
    /// </summary>
    public Node GetNode(string nodeID)
    {
        if (nodeHasCreated.TryGetValue(nodeID,out Node node))
        {
            return node;
        }
        else
        {
            Debug.Log($"No this node {nodeID} in nodeMap");
            return null;  
        }
    }

    /// <summary>
    /// 删除当前节点图中的所有节点以及对应绑定的线条
    /// </summary>
    public void DeleteNodeMap()
    {
        foreach (KeyValuePair<string,Node> keyValue in nodeHasCreated)
        {
            // 删除节点对象
            Node currentNode = keyValue.Value;

            if (currentNode.nodeType.isControllable)
            {
                Controllable controllable = currentNode.GetComponent<Controllable>();
                Destroy(controllable.line);
                Destroy(controllable.hammer);
            }

            Destroy(currentNode.gameObject);

            // 删除节点对象对应的线条对象
            if (LineCreator.Instance.nodeLineBinding.TryGetValue(currentNode,out Line line))
            {
                Destroy(line.gameObject);
            }
        }
    }

    /// <summary>
    /// 保存整个节点图，包含所有节点的节点状态
    /// </summary>
    public void SaveNodeMap(List<string> nodeSaveIDList)
    {
        foreach (KeyValuePair<string,Node> keyValuePair in nodeHasCreated)
        {
            Node currentNode = keyValuePair.Value;
            string nodeID = keyValuePair.Key;

            NodeState nodeState = new NodeState{
                nodeType = currentNode.nodeType,
                localPosition = currentNode.transform.position,
                childNodeID = currentNode.childIdList,
                parentNodeID = currentNode.parentID,
                hasPopUp = currentNode.hasPopUp,
                isActive = currentNode.gameObject.activeSelf
            };
            SaveProfile<NodeState> saveProfile = new SaveProfile<NodeState>(nodeID,nodeState);
            SaveManager.Delete(saveProfile.profileName);
            SaveManager.Save(saveProfile);

            if (!nodeSaveIDList.Contains(nodeID))
                nodeSaveIDList.Add(nodeID);
        }
    }

    /// <summary>
    /// 加载整个节点图，包含所有节点的节点状态
    /// </summary>
    public void LoadNodeMap(List<string> nodeSaveIDList)
    {
        foreach (string nodeIDHasSave in nodeSaveIDList)
        {
            NodeState nodeState = SaveManager.Load<NodeState>(nodeIDHasSave).saveData;

            nodeHasCreated[nodeIDHasSave].transform.localPosition = nodeState.localPosition;
            nodeHasCreated[nodeIDHasSave].childIdList = nodeState.childNodeID;
            nodeHasCreated[nodeIDHasSave].parentID = nodeState.parentNodeID;
            nodeHasCreated[nodeIDHasSave].hasPopUp = nodeState.hasPopUp;
            if (nodeState.hasPopUp)
            {
                foreach (string childNodeID in nodeState.childNodeID)
                {
                    LineCreator.Instance.ShowLine(GetNode(childNodeID));
                }
            }
            nodeHasCreated[nodeIDHasSave].gameObject.SetActive(nodeState.isActive);
        }

        LocateCameraAtEntranceNode();
    }

}
