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
        
        myNode = transform.GetComponent<Node>();
    }

    private void Start() {
        targetNode = NodeMapBuilder.Instance.GetNode(targetNodeID);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (targetNode != null)
        {
            if (hasSynthesized || targetNode.isPopping || myNode.isPopping || targetNode.isDragging || myNode.isDragging) return;

            if (collision.transform.GetComponent<Node>() == targetNode)
            {
                MergeTowNode();

                hasSynthesized = true;
            }
        }
        else
        {
            Debug.Log($"{name} have no target Node");
        }
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
            UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }


    private void MergeTowNode()
    {
        targetNode.gameObject.SetActive(false);
        LineCreator.Instance.DeleteLine(targetNode);

        myNode.PopUpChildNode(myNode.nodeInfos);
        myNode.hasPopUp = true;
    }
}
