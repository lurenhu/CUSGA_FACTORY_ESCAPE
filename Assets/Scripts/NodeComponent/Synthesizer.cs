using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{   
    [Header("观测参数")]
    public Node targetNode;
    private string targetNodeID;
    private Node myNode;
    private bool hasSynthesized = false;
    
    public void InitializeSynthesizer(NodeSO nodeSO) 
    {
        SynthesizableNodeSO synthesizableNodeSO = (SynthesizableNodeSO)nodeSO;

        targetNodeID = synthesizableNodeSO.targetIdForMerge;
    }

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (targetNode == null && NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(targetNodeID, out Node targetNodeTemp))
            targetNode = targetNodeTemp;

        if (targetNode != null)
        {
            if (hasSynthesized || targetNode.isPopping || myNode.isPopping) return;

            if (collision.transform.GetComponent<Node>() == targetNode)
            {
                MergeTowNode();

                hasSynthesized = true;
            }
        }
    }

    private void MergeTowNode()
    {
        targetNode.gameObject.SetActive(false);

        myNode.PopUpChildNode(myNode.nodeInfos);
    }
}
