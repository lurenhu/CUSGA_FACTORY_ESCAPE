using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SynthesizableNode : Node
{
    public bool hasSynthesized = false;
    public Node currentNode;

    private void OnTriggerStay2D(Collider2D collision) {
        Debug.Log("onTrigger");
        if (hasSynthesized) return;
        currentNode = collision.GetComponent<Node>();

        if (!currentNode.isDragging && !currentNode.isPopping)
        {
            MergeTwoNode();
        }
    }

    /// <summary>
    /// 合成两个节点，生成子节点
    /// </summary>
    private void MergeTwoNode()
    {
        NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(nodeProperty.targetNodeID,out Node targetNode);

        if (targetNode == currentNode)
        {
            Destroy(targetNode.transform.gameObject);
    
            PopUpChildNode(nodeInfos);
        }
    }
}
