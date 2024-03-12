using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    public Node targetNode;
    public Node myNode;
    
    private bool hasSynthesized = false;
    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (hasSynthesized || targetNode.isPopping || myNode.isPopping) return;

        if (collision.transform.GetComponent<Node>() == targetNode)
        {
            MergeTowNode();

            hasSynthesized = true;
        }

    }

    private void MergeTowNode()
    {
        targetNode.gameObject.SetActive(false);

        myNode.PopUpChildNode(myNode.nodeInfos);
    }
}
