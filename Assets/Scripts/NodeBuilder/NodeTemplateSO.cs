using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NodeTempateSO_",menuName = "ScriptableObjects/NodeTemplate")]
public class NodeTemplateSO : ScriptableObject
{
    public string guid;
    public GameObject nodePrefab;
    [HideInInspector] public GameObject previousPrefab;
    public NodeTypeSO nodeType;

    #if UNITY_EDITOR
    private void OnValidate() {
        if (guid == "" || previousPrefab != nodePrefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = nodePrefab;
            EditorUtility.SetDirty(this);
        }
    }
    #endif
}
