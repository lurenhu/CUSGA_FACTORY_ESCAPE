using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public NodeTypeListSO nodeTypeList;

    public List<Character> characters = new List<Character>();
}

[System.Serializable]
public class Character
{
    public string name;
    public Sprite sprite;
}
