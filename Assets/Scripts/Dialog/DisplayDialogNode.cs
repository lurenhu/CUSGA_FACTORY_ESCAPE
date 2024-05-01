using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDialogNode : MonoBehaviour
{
    public Node timingNode;

    public void InitializeBeClocked(Node timingNode)
    {
        this.timingNode = timingNode;
    }

    private void Update() {
        if (timingNode != null && !timingNode.gameObject.activeSelf)
        {
            timingNode.gameObject.SetActive(true);
        }
    }
}
