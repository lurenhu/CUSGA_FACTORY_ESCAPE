using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SynthesizableNode : Node
{
    private bool hasSynthesizabled = false;
    private bool hasInTrigger = false;
    private Node currentNode;
    public Node targetNode;

    protected override void OnMouseUp() {
        base.OnMouseUp();

        if (isSelected)
        {
            Debug.Log("Synthesizable Node get mission");

            if (hasPopUp) return;

            if (hasInTrigger)
            {
                MergeTwoNode();
            }
        }
        else
        {
            isSelected = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (hasSynthesizabled) return;
        if (hasInTrigger) hasInTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (hasSynthesizabled) return;

        currentNode = collision.GetComponent<Node>();
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (hasSynthesizabled) return;
        if (hasInTrigger) hasInTrigger = false;
    }

    /// <summary>
    /// 合成两个节点，生成子节点
    /// </summary>
    private void MergeTwoNode()
    {
        if (targetNode == currentNode)
        {
            Destroy(targetNode.transform.gameObject);
    
            PopUpChildNode(nodeInfos);

            hasPopUp = true;
            hasSynthesizabled = true;
        }
    }
}
