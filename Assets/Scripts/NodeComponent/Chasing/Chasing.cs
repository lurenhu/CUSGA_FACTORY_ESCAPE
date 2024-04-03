using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasing : MonoBehaviour
{
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    public void InitializeChasingNode(NodeSO nodeSO)
    {
        ChasingNodeSO chasingNodeSO = (ChasingNodeSO)nodeSO;
    }
}
