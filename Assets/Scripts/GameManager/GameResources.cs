using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Video;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }
    
    [Header("游戏资源")]

    [Tooltip("节点类型列表")]
    public NodeTypeListSO nodeTypeList;

    [Tooltip("角色列表(对话系统)")]
    public List<Character> characters = new List<Character>();

    [Tooltip("QTE图片")]
    public List<QTESprite> QTESprites = new List<QTESprite>();
    
    [Tooltip("AI模板文本")]
    public List<TextAsset> botTextAsset = new List<TextAsset>();

}

[System.Serializable]
public class Character
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class CutSceneCell
{
    public string animationStateName;
    public TextAsset text;
    public bool isAuto;
    public AudioClip music;
    public AudioClip sfx;
}

[System.Serializable]
public class QTESprite
{
    public Direction direction;
    public Sprite sprite;
}

