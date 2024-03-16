using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDetector : MonoBehaviour
{
    private float speedToPop;
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    public void SetSpeedToPop(float speed) {
        speedToPop = speed;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        float speed = collision.relativeVelocity.magnitude;        

        if (speed > speedToPop && !myNode.hasPopUp)
        {
            myNode.PopUpChildNode(myNode.nodeInfos);
            myNode.hasPopUp = true;
        }
    }
}
