using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeTypeList_",menuName = "ScriptableObjects/NodeTypeList")]
public class NodeTypeListSO : ScriptableObject
{
    public List<NodeTypeSO> list;
}
