using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeTempateSO_",menuName = "ScriptableObjects/NodeTemplate")]
public class NodeTemplateSO : ScriptableObject
{
    public GameObject nodePrefab;
    public NodeTypeSO nodeType;
}
