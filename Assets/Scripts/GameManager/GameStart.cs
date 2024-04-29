using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : SingletonMonobehaviour<GameStart>
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
        soundManager.Instance.StopMusicInFade();
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        GameManager.Instance.canvasGroup.blocksRaycasts = false;
        yield return StartCoroutine(GameManager.Instance.Fade(0,1,2,Color.black));

        soundManager.Instance.PlaySFX("ChangeScene");

        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadSceneAsync("GameScene",LoadSceneMode.Additive);

        GameManager.Instance.gameState = GameState.Generating;
    }



    public void QuitGame()
    {
        Application.Quit();
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
