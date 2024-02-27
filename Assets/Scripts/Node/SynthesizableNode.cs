using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SynthesizableNode : Node
{
    private bool hasSynthesizabled = false;
    public Node targetNode;

    private void OnMouseUp() {
        if (isSelected)
        {
            if (isPoping || hasPopUp) return;
            Debug.Log("Synthesizable Node get mission");
        }
        else
        {
            isSelected = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        Debug.Log("On trigger");
        if (hasSynthesizabled) return;

        if (Input.GetMouseButtonUp(0) && targetNode == collision.transform.GetComponent<Node>())
        {
            MergTwoNode();
            hasSynthesizabled = true;
        }
    }

    /// <summary>
    /// 合成两个节点，生成子节点
    /// </summary>
    private void MergTwoNode()
    {
        Destroy(targetNode.transform.gameObject);

        PopUpChildNode(childNodes);

        hasPopUp = true;
    }
}
