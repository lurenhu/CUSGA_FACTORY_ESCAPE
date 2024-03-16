using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeClocked : MonoBehaviour
{
    public Node timingNode;

    public void InitializeBeClocked(Node timingNode)
    {
        Debug.Log(1);
        this.timingNode = timingNode;
    }

    private void Update() {
        if (timingNode != null && !timingNode.gameObject.activeSelf)
        {
            timingNode.gameObject.SetActive(true);
            enabled = false;
        }
    }
    
}