using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    public string startTimingNodeId;
    public string endTimingNodeId;

    public Node startTimingNode;
    public Node endTimingNode;

    public Node timingNode;

    bool getMatch = false;
    public bool startTimer = false; 

    private void Update() {
        if (startTimingNode == null && startTimingNodeId != null)
        {
            startTimingNode = NodeMapBuilder.Instance.GetNode(startTimingNodeId);
        }

        if (endTimingNode == null && endTimingNodeId != null)
        {
            endTimingNode = NodeMapBuilder.Instance.GetNode(endTimingNodeId);
        }

        if (!getMatch && startTimingNode != null && endTimingNode != null)
        {
            BeClocked beClocked = startTimingNode.gameObject.AddComponent<BeClocked>();
            beClocked.InitializeBeClocked(timingNode);
            StopClocked stopClocked = endTimingNode.gameObject.AddComponent<StopClocked>();
            stopClocked.InitializeStopClocked(timingNode);
            getMatch = true;
        }

        if (startTimer && timingNode.gameObject.activeSelf)
        {
            timingNode.GetComponent<Timing>().StartTimerCoroutine();
            startTimer = false;
        }
    }
}
