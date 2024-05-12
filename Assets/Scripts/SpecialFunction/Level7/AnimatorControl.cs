using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    Node myNode;

    private void Start() {
        myNode = GetComponent<Node>();
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
                    UIManager.Instance.pauseButton.gameObject.SetActive(false);
                    UIManager.Instance.AnimatorUI.gameObject.SetActive(true);

                    soundManager.Instance.StopMusicInFade();
                    soundManager.Instance.PlaySFX(myNode.audios[0]);

                    StartCoroutine(myNode.PopUpChildNodes(myNode.nodeInfos));
                    myNode.hasPopUp = true;
                    return;
                }
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }
}
