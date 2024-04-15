using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class StartSceneNode : MonoBehaviour
{
    private Node myNode;

    public UnityEvent OnTrigger;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
    
                // 弹出子节点
                if (!myNode.hasPopUp && myNode.nodeInfos.Count != 0)
                {
                    PopUpChildNode(myNode.nodeInfos);
                    myNode.hasPopUp = true;
                    return;
                }

                if (OnTrigger != null)
                {
                    OnTrigger.Invoke();
                }

            }
            else
            {
                // 删除其他所有节点的选中状态
                GameStart.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            // 播放音频
            if (myNode.audios.Count != 0)
            {
                soundManager.Instance.PlayMusic(myNode.audios[0]);
            }
            // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
            // UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }


    private void PopUpChildNode(List<NodeInfo> nodeInfos)
    {
        foreach (NodeInfo childNode in nodeInfos)
        {
            Node currentNode = childNode.node; // Instantiate(childNode.node,transform.position,Quaternion.identity);

            currentNode.transform.position = transform.position;
            currentNode.gameObject.SetActive(true);

            GameStart.Instance.CreateLine(currentNode);

            currentNode.transform.DOMove(
                childNode.direction * GameManager.Instance.popUpForce,GameManager.Instance.tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentNode.isPopping = true;
                }).OnComplete(() => 
                {
                    currentNode.isPopping = false;
                });
        }
    }
}
