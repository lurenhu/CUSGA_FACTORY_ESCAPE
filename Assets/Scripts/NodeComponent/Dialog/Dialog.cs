using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    [Space(5)]
    [Header("观测数据")]
    [Tooltip("文本文件")]
    public List<TextAsset> textAssets;
    [Tooltip("是否再对话结束后替换子节点")]
    public bool stopAfterDialog;
    private Node myNode;
    private Node parentNode;
    private int dialogIndex = 0;
    private Sprite changeBackGroundImage;

    private void Start() {
        parentNode = NodeMapBuilder.Instance.GetNode(myNode.parentID);
    }

    /// <summary>
    /// 传递并初始化对话节点数据
    /// </summary>
    public void InitializeDialog(NodeSO nodeSO)
    {
        myNode = transform.GetComponent<Node>();

        DialogNodeSO dialogNodeSO = (DialogNodeSO)nodeSO;

        textAssets = dialogNodeSO.textAssets;
        stopAfterDialog = dialogNodeSO.stopAfterDialog;
        changeBackGroundImage = dialogNodeSO.changeBackGroundImage;

        if (NodeMapBuilder.Instance.GetNode(dialogNodeSO.DisplayNodeID) != null && NodeMapBuilder.Instance.GetNode(dialogNodeSO.DisappearNodeID) != null)
        {
            BeClocked beClocked = NodeMapBuilder.Instance.GetNode(dialogNodeSO.DisplayNodeID).gameObject.AddComponent<BeClocked>();
            beClocked.InitializeBeClocked(myNode);
            StopClocked stopClocked = NodeMapBuilder.Instance.GetNode(dialogNodeSO.DisappearNodeID).gameObject.AddComponent<StopClocked>();
            stopClocked.InitializeStopClocked(myNode);
        }
    }

    private void OnMouseUp() {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                if (changeBackGroundImage != null)
                    StartCoroutine(ChangingBackGroundImage(changeBackGroundImage));

                // 节点交互内容
                if (stopAfterDialog)
                {
                    ExchangeToChildNodesAfterDialog();
                }         
                else
                {
                    KeepDialog();
                    if (!myNode.hasPopUp)
                    {
                        StartCoroutine(myNode.PopUpChildNode(myNode.nodeInfos));
                        myNode.hasPopUp = true;
                    }
                }
    
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }

    private IEnumerator ChangingBackGroundImage(Sprite sprite)
    {
        yield return StartCoroutine(UIManager.Instance.Fade(1,0,2,Color.white));

        UIManager.Instance.backGround.GetComponent<Image>().sprite = sprite;

        yield return StartCoroutine(UIManager.Instance.Fade(0,1,2,Color.white));
    }

    /// <summary>
    /// 持续对话
    /// </summary>
    private void KeepDialog()
    {
        if (dialogIndex < textAssets.Count)
        {
            DialogSystem.Instance.GetText(textAssets[dialogIndex]);
            dialogIndex++;
        }
        else if (dialogIndex == textAssets.Count)
        {
            DialogSystem.Instance.GetText(textAssets[dialogIndex - 1]);
        }
    }

    /// <summary>
    /// 在对话文本结束后将当前节点替换为其子节点
    /// </summary>
    private void ExchangeToChildNodesAfterDialog()
    {
        if (dialogIndex < textAssets.Count)
        {
            DialogSystem.Instance.GetText(textAssets[dialogIndex]);
            dialogIndex++;

            if (dialogIndex == textAssets.Count)
            {
                parentNode.childIdList.Clear();

                // 消除当前节点
                LineCreator.Instance.DeleteLine(myNode);
                myNode.gameObject.SetActive(false);

                // 将当前节点的所有子节点的父节点设置为当前节点的父节点
                foreach (string childNodeId in myNode.childIdList)
                {
                    Node childeNode = NodeMapBuilder.Instance.GetNode(childNodeId);
                    Destroy(LineCreator.Instance.GetLine(childeNode));
                    LineCreator.Instance.nodeLineBinding.Remove(childeNode);

                    childeNode.parentID = parentNode.id;
                    parentNode.childIdList.Add(childNodeId);
                    LineCreator.Instance.CreateLine(childeNode);
                }
                StartCoroutine(parentNode.PopUpChildNode(myNode.nodeInfos));
            }
        }
        
        
    }
}
