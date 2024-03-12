using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeType_",menuName = "ScriptableObjects/NodeType")]
public class NodeTypeSO : ScriptableObject
{
    public string nodeTypeName;

    public bool isEntrance;
    public bool isDefault;
    public bool isSynthetic;
    public bool isLocked;
    public bool isAI;
    public bool isGraph;
    public bool isAngleLock;
    public bool isProbe;
    public bool isControllable;
}
