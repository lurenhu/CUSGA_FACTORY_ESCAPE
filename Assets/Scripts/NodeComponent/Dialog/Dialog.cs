using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    [Space(5)]
    [Header("观测数据")]
    private Node myNode;
    public List<TextAsset> textAssets;
    public bool stopAfterDialog;
    private int dialogIndex = 0;

    private void Start() {
        myNode = transform.GetComponent<Node>();
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
        if (myNode.isPopping) return;

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

        if (myNode.isDragging) myNode.isDragging = false;
    }

    /// <summary>
    /// 持续对话
    /// </summary>
    private void KeepDialog()
    {
        if (dialogIndex < textAssets.Count)
        {
            DialogSystem.GetText(textAssets[dialogIndex]);
            dialogIndex++;
        }
        else if (dialogIndex == textAssets.Count)
        {
            DialogSystem.GetText(textAssets[dialogIndex - 1]);
        }
    }

    /// <summary>
    /// 在对话文本结束后将当前节点替换为其子节点
    /// </summary>
    private void ExchangeToChildNodesAfterDialog()
    {
        if (dialogIndex < textAssets.Count)
        {
            DialogSystem.GetText(textAssets[dialogIndex]);
            dialogIndex++;

            if (dialogIndex == textAssets.Count)
            {
                // 消除当前节点
                LineCreator.Instance.DeleteLine(myNode);
                myNode.gameObject.SetActive(false);

                // 清除当前节点父节点的子节点集
                Node parentNode = NodeMapBuilder.Instance.nodeHasCreated[myNode.parentID];

                // 将当前节点的所有子节点的父节点设置为当前节点的父节点
                foreach (string childNodeId in myNode.childIdList)
                {
                    Node childeNode = NodeMapBuilder.Instance.nodeHasCreated[childNodeId];
                    LineCreator.Instance.DeleteLine(childeNode);

                    childeNode.parentID = parentNode.id;
                    LineCreator.Instance.CreateLine(childeNode);
                }

                parentNode.PopUpChildNode(myNode.nodeInfos);
            }
        }
        
        
    }
}
