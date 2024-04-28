using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : SingletonMonobehaviour<GamePause>
{
    public List<Node> startNodes = new List<Node>();
    public GameObject LinePrefab;

    public void ClearAllSelectedNode(Node node)
    {
        foreach (Node currentNode in startNodes)
        {
            if (currentNode != node && currentNode.isSelected)
            {
                currentNode.isSelected = false;
                currentNode.GetUnSelectedAnimate();
            }
        }
    }

    public void CreateLine(Node node)
    {
        GameObject line = Instantiate(LinePrefab, transform.position, Quaternion.identity,transform);

        Line lineComponent = line.GetComponent<Line>();

        Node parentNode = null;

        foreach (Node currentNode in startNodes)
        {
            if (currentNode.id == node.parentID)
            {
                parentNode = currentNode;
            }
        }

        if (parentNode != null)
        {
            lineComponent.InitializeLine(node.transform,parentNode.transform);
            Debug.Log($"{node.name} has create Line");
        }

    }

    public void Continue()
    {   
        SceneManager.UnloadSceneAsync("PauseMenu");
    }


    public void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
