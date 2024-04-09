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
        if (myNode.isDragging) myNode.isDragging = false;

        ProcessCipherClickEvent();
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
