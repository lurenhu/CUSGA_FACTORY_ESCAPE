using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NodeGraph_",menuName = "ScriptableObjects/NodeGraph")]
public class NodeGraphSO : ScriptableObject
{
    public string graphName;
    [HideInInspector] public NodeTypeListSO nodeTypeList;
    [HideInInspector] public List<NodeSO> nodeList = new List<NodeSO>();
    [HideInInspector] public Dictionary<string,NodeSO> nodeDictionary = new Dictionary<string, NodeSO>();

    private void Awake()
    {
        LoadNodeDictionary();
    }

    private void LoadNodeDictionary()
    {
         nodeDictionary.Clear();

        foreach ( NodeSO node in nodeList)
        {
            nodeDictionary[node.id] = node;
        }
    }

    public NodeSO GetNode( NodeTypeSO nodeType)
    {
        foreach ( NodeSO node in nodeList)
        {
            if ( node.nodeType == nodeType)
            {
                return node;
            }
        }
        return null;
    }

    public NodeSO GetNode(string nodeID)
    {
        if (nodeDictionary.TryGetValue(nodeID,out NodeSO node))
        {
            return node;
        }
        return null;
    }

    public IEnumerable<NodeSO> GetChildNodes( NodeSO parentNode)
    {
        foreach (string childNodeID in parentNode.childrenNodeIdList)
        {
            yield return GetNode(childNodeID);
        }
    }

#if UNITY_EDITOR
    [HideInInspector] public NodeSO nodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate() {
        LoadNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(NodeSO node, Vector2 position)
    {
        nodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif
}
