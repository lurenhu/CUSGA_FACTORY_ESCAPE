using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CipherNode : Node
{
    [Space(10)]
    [Header("CIPHER NODE")]
    public int index;// 索引
    public int value = 0;// 值

    protected override void OnMouseUp()
    {
        base.OnMouseUp();
        if (isSelected)
        {
            ProcessCipherClickEvent();
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }

    private void ProcessCipherClickEvent()
    {
        if (value == 9)
            value = 1;
        else
            value++;
    }

    public void InitializeCipherNode(int index, int value)
    {
        this.index = index;
        this.value = value;
    }
}
