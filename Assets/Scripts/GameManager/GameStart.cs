using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : SingletonMonobehaviour<GameStart>
{
    public Transform loadPanel;
    public Slider loadSlider;
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

    public void StartGame()
    {   
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        loadPanel.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync("NodeMapTest");

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadSlider.value = operation.progress;

            if (operation.progress >= 0.9)
            {
                loadSlider.value = 1;

                yield return new WaitForSeconds(1);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        GameManager.Instance.gameState = GameState.Generating;
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
