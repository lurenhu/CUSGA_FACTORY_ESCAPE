using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeLevel_",menuName = "ScriptableObjects/NodeLevel")]
public class NodeLevelSO : ScriptableObject
{
    public string levelName;

    public List<NodeGraphSO> levelGraphs;

}
