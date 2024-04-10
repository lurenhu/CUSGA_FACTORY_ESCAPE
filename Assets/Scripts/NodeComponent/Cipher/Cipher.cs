using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cipher : MonoBehaviour
{
    [HideInInspector] public Node myNode;

    [Header("观察参数")]
    public int index;// 索引
    public int value = 0;// 值

    private void OnMouseUp() {
        if (myNode.isPopping) return;
        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                ProcessCipherClickEvent();
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            // 播放音频
            if (myNode.audios.Count != 0)
            {
                soundManager.Instance.PlayMusic(myNode.audios[0]);
            }
            // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
            UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        }
    }

    private void ProcessCipherClickEvent()
    {
        if (value == 9)
            value = 0;
        else
            value++;

        transform.Find("Value").GetComponent<TMP_Text>().text = value.ToString();
    }

    public void InitializeCipherNode(int index, int value)
    {
        this.index = index;
        this.value = value;
        myNode = transform.GetComponent<Node>();

        transform.Find("Value").GetComponent<TMP_Text>().text = value.ToString();
        transform.Find("Index").GetComponentInChildren<TMP_Text>().text = (index + 1).ToString();
    }
}
