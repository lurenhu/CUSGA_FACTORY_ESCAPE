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
    /// 创建节点
    /// </summary>
    public void CreateLine(Node node)
    {
        GameObject line = Instantiate(LinePrefab, transform.position, Quaternion.identity,transform);

        Line lineComponent = line.GetComponent<Line>();

        NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(node.nodeProperty.parentID, out Node parentNode);

        lineComponent.InitializeLine(node.transform,parentNode.transform);

        nodeLineBinding.Add(node, lineComponent);
    }

    public void CreateLine(Transform startPoint, Transform endPoint)
    {
        GameObject line = Instantiate(LinePrefab, transform.position, Quaternion.identity,transform);

        Line lineComponent = line.GetComponent<Line>();

        lineComponent.InitializeLine(startPoint, endPoint);
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    public void DeleteLine(Node node)
    {
        Line line = nodeLineBinding.TryGetValue(node, out Line lineComponent) ? lineComponent : null;

        if (line != null)
        {
            Destroy(line.gameObject);
        }
    }

}
