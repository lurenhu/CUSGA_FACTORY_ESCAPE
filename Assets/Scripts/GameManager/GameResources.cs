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

    [Tooltip("音乐列表")]
    public List<Sound> musics= new List<Sound>();
    [Tooltip("音效列表")]
    public List<Sound> SFXs = new List<Sound>();

    [Tooltip("视频列表")]
    public List<Video> videos = new List<Video>();

    [Tooltip("QTE图片")]
    public List<QTESprite> QTESprites = new List<QTESprite>();

}

[System.Serializable]
public class Character
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class Video
{
    public string name;
    public VideoClip videoClip;
}

[System.Serializable]
public class CutSceneCell
{
    public string animationStateName;
    public TextAsset text;
    public bool isAuto;
}

[System.Serializable]
public class QTESprite
{
    public Direction direction;
    public Sprite sprite;
}

