using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopClocked : MonoBehaviour
{
    public Node timingNode;

    public void InitializeStopClocked(Node timingNode)
    {
        this.timingNode = timingNode;
    }

    private void Update() {
        if (timingNode != null && timingNode.gameObject.activeSelf)
        {
            timingNode.gameObject.SetActive(false);
        }
    }



}
