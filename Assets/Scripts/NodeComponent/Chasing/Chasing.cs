using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasing : MonoBehaviour
{
    public float moveSpeed = 3;

    private Node myNode;

    private string chasingTargetId;
    private Node chasingTargetNode;


    public void InitializeChasingNode(NodeSO nodeSO)
    {
        ChasingNodeSO chasingNodeSO = (ChasingNodeSO)nodeSO;

        chasingTargetId = chasingNodeSO.chasingTargetId;

        myNode = transform.GetComponent<Node>();
    }

    private void Start() {
        chasingTargetNode = NodeMapBuilder.Instance.GetNode(chasingTargetId);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (chasingTargetNode == null) return;

        if (collider.GetComponent<Node>() == chasingTargetNode)
        {
            Debug.Log("be caught");
        }
    }

    private void Update() {
        if (chasingTargetNode == null || myNode.isPopping) return;

        Vector2 direction = (chasingTargetNode.transform.position - myNode.transform.position).normalized;

        // gameObject.transform.Translate()
    }


}
