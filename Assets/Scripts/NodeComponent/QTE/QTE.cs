using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class QTE : MonoBehaviour
{
    [Header("观测数据")]
    public Node myNode;

    [Space(5)]
    [Header("可调整数据")]
    public Vector2 dragStartPosition;
    public Vector2 dragDirection;
    private float dragDistanceThreshold;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    /// <summary>
    /// 传递QTE数据
    /// </summary>
    public void InitializeQTE(NodeSO nodeSO)
    {
        QTENodeSO qTENodeSO = (QTENodeSO)nodeSO;

        dragDistanceThreshold = qTENodeSO.dragDistance;
        
        switch (qTENodeSO.direction)
        {
            case Direction.Up:
                dragDirection = Vector2.up;
                break;
            case Direction.Down:
                dragDirection = Vector2.down;
                break;
            case Direction.Left:
                dragDirection = Vector2.left;
                break;
            case Direction.Right:
                dragDirection = Vector2.right;
                break;
        }
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            // 节点交互内容
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }

    private void Update() {
        // 鼠标左键按下开始拖动
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse down");
            dragStartPosition = Input.mousePosition;
        }

        // 鼠标左键持续按下时进行拖动判断
        if (myNode.isDragging && Input.GetMouseButton(0) && !myNode.hasPopUp)
        {
            Debug.Log("Mouse dragging");
            Vector2 dragCurrentPosition = Input.mousePosition;
            float dragDistance = Vector2.Dot(dragCurrentPosition - dragStartPosition, dragDirection);

            // 如果拖动距离超过阈值，则触发回调函数
            if (dragDistance >= dragDistanceThreshold)
            {
                myNode.PopUpChildNode(myNode.nodeInfos);
                myNode.hasPopUp = true;
            }
        }
    }

}
