using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeTypeList",menuName = "ScriptableObjects/NodeTypeList_")]
public class NodeTypeListSO : ScriptableObject
{
    public List<NodeTypeSO> list;
}
