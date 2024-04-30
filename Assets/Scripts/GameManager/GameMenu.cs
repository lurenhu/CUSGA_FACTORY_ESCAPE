using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : SingletonMonobehaviour<GameMenu>
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

    public void StartGame()
    {   
        GameManager.Instance.levelIndex = 0;
        GameManager.Instance.StartChangeSceneCoroutine("MainMenu","GameScene",GameState.Generating);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Continue()
    {   
        GameManager.Instance.UnloadGameScene("PauseMenu");
    }

    public void PauseBackMain()
    {
        GameManager.Instance.StartChangeSceneCoroutine("PauseMenu","MainMenu",GameState.Start);
    }

    public void ReStart()
    {
        GameManager.Instance.StartChangeSceneCoroutine("FailMenu","GameScene",GameState.Generating);
    }

    public void FailBackMain()
    {
        GameManager.Instance.StartChangeSceneCoroutine("FailMenu","MainMenu",GameState.Start);
    }

    public void ChangeMusicVolume(float volume)
    {
        soundManager.Instance.setMusicVolume(volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        soundManager.Instance.setSfxVolume(volume);
    }
}
