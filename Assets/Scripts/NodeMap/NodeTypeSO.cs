using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeType_",menuName = "ScriptableObjects/NodeType")]
public class NodeTypeSO : ScriptableObject
{
    public string nodeTypeName;

    public bool isDefault;
    public bool isSynthesizable;
    public bool isLocked;
    public bool isCipher;
    public bool isAI;
}
