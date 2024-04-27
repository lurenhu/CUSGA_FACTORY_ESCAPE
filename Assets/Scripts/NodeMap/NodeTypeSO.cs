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
    public bool isSyntheticPicture;
    public bool isTiming;
    public bool isQuickClick;
    public bool isQTE;
    public bool isDialog;
    public bool isText;
    public bool isChasing;
    public bool isExit;
    
    [Space(10)]
    [Header("关卡1")]
    public bool isAIAnxietyChanged;
    public bool isLevel1AILock;

     [Space(10)]
    [Header("关卡3")]
    public bool isMoving;

}
