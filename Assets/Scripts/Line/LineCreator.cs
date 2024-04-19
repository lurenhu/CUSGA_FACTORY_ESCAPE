using System.Collections.Generic;
using UnityEngine;

public class LineCreator : SingletonMonobehaviour<LineCreator>
{
    public GameObject LinePrefab;
    public Dictionary<Node,Line> nodeLineBinding = new Dictionary<Node, Line>();

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 创建节点线条
    /// </summary>
    public void CreateLine(Node node)
    {
        Node parentNode = NodeMapBuilder.Instance.GetNode(node.parentID);
        if (parentNode == null) return;
        
        GameObject line = Instantiate(LinePrefab, transform.position, Quaternion.identity,transform);

        line.SetActive(false);

        Line lineComponent = line.GetComponent<Line>();

        lineComponent.InitializeLine(node.transform,parentNode.transform);

        nodeLineBinding.Add(node, lineComponent);
    }

    /// <summary>
    /// 根据位置创建线条
    /// </summary>
    public GameObject CreateLine(Transform startPoint, Transform endPoint)
    {
        GameObject line = Instantiate(LinePrefab, transform.position, Quaternion.identity,transform);

        Line lineComponent = line.GetComponent<Line>();

        lineComponent.InitializeLine(startPoint, endPoint);

        return line;
    }

    /// <summary>
    /// 删除所有线条对象
    /// </summary>
    public void DeleteAllLine()
    {
        foreach (KeyValuePair<Node,Line> keyValuePair in nodeLineBinding)
        {
            Line currentLine = keyValuePair.Value;
            Destroy(currentLine.gameObject);
        }

        nodeLineBinding.Clear();
    }

    /// <summary>
    /// 将所创建的节点线条展现
    /// </summary>
    public void ShowLine(Node node)
    {
        Line line = nodeLineBinding.TryGetValue(node, out Line lineComponent) ? lineComponent : null;

        if (line != null)
        {
            line.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 删除节点线条
    /// </summary>
    public void DeleteLine(Node node)
    {
        Line line = nodeLineBinding.TryGetValue(node, out Line lineComponent) ? lineComponent : null;

        if (line != null)
        {
            line.gameObject.SetActive(false);
        }
    }

    public Line GetLine(Node node)
    {
        if (nodeLineBinding.TryGetValue(node,out Line line))
        {
            return line;
        }
        else
        {
            Debug.Log($"No Match Node Line For {node}");
            return null;
        }

    }

}
