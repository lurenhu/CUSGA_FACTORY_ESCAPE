using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

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

    public void ContinueFromMain(Node targetNode)
    {
        if (GameManager.Instance.levelIndex == -1)
        {
            Debug.Log("无游戏存档");
            NoGameArchive(targetNode);
            return;
        }
        GameManager.Instance.ChangeAndLoadGameScene("MainMenu");
    }

    public void ContinueByLoad()
    {
        GameManager.Instance.ChangeAndLoadGameScene("PauseMenu");
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

    private void NoGameArchive(Node currentNode)
    {   
        Node parentNode = startNodes.Find(x => x.id == currentNode.parentID);

        currentNode.transform.position = parentNode.transform.position;
        currentNode.transform.localScale = Vector3.one * 0.3f; 
        currentNode.gameObject.SetActive(true);

        Instance.CreateLine(currentNode);
        soundManager.Instance.PlaySFX("NodeBorn");

        Sequence sequence = DOTween.Sequence();
        sequence.Append(currentNode.transform.DOMove(
            new Vector2(-0.4f,0.4f) * GameManager.Instance.popUpForce,GameManager.Instance.tweenDuring
            ).SetRelative().OnStart(() => 
            {
                currentNode.isPopping = true;
            }).OnComplete(() => 
            {
                currentNode.isPopping = false;
            }));
            
        sequence.Append(currentNode.transform.DOScale(new Vector3(1f,0.3f,1),0.1f));
        sequence.Append(currentNode.transform.DOScale(new Vector3(1f,1f,1),0.1f));
    }
}
