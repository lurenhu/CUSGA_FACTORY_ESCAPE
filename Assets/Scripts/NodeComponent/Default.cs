using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default : MonoBehaviour
{
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            // 节点交互内容
            // 对话
            if (myNode.dialogTexts.Count > myNode.dialogTextsIndex)
            {
                DialogManager.Instance.LoadDialogData(myNode.dialogTexts[myNode.dialogTextsIndex]);
                myNode.dialogTextsIndex++;
                return;
            }

            // 弹出子节点
            if (!myNode.hasPopUp)
            {
                myNode.PopUpChildNode(myNode.nodeInfos);
                myNode.hasPopUp = true;
                return;
            }

            // 播放音频
            soundManager.playSFX(myNode.audios[0]);            
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }
}
