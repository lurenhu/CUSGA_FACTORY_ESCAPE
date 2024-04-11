using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start() {
        myNode = transform.GetComponent<Node>();

        parentNode = NodeMapBuilder.Instance.nodeHasCreated[myNode.parentID];
    }

    /// <summary>
    /// 传递并初始化对话节点数据
    /// </summary>
    public void InitializeDialog(NodeSO nodeSO)
    {
        DialogNodeSO dialogNodeSO = (DialogNodeSO)nodeSO;

        textAssets = dialogNodeSO.textAssets;
        stopAfterDialog = dialogNodeSO.stopAfterDialog;
    }

    private void OnMouseUp() {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
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
                        myNode.PopUpChildNode(myNode.nodeInfos);
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
                // 消除当前节点
                LineCreator.Instance.DeleteLine(myNode);
                myNode.gameObject.SetActive(false);

                // 将当前节点的所有子节点的父节点设置为当前节点的父节点
                foreach (string childNodeId in myNode.childIdList)
                {
                    Node childeNode = NodeMapBuilder.Instance.nodeHasCreated[childNodeId];
                    LineCreator.Instance.DeleteLine(childeNode);
                    LineCreator.Instance.nodeLineBinding.Remove(childeNode);

                    childeNode.parentID = parentNode.id;
                    LineCreator.Instance.CreateLine(childeNode);
                }
                parentNode.PopUpChildNode(myNode.nodeInfos);
            }
        }
        
        
    }
}
