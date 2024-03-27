using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

public class QTE : MonoBehaviour
{
    [Header("观测数据")]
    public Node myNode;
    private Vector2 dragStartPosition;
    public float dragDistanceThreshold;
    private Direction direction;

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

        direction = qTENodeSO.direction;
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
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
            Vector2 dragCurrentPosition = Input.mousePosition;

            Vector2 dragDistance = dragCurrentPosition - dragStartPosition;

            Direction currentDirection = CheckDirection(dragDistance);

            //如果拖动距离超过阈值，则触发回调函数
            if (dragDistance.magnitude >= dragDistanceThreshold)
            {
                if (currentDirection == direction)
                {
                    myNode.PopUpChildNode(myNode.nodeInfos);
                    myNode.hasPopUp = true;
                }
                else
                {
                    StaticEventHandler.CallStopTiming(myNode);
                } 
            }
        }
    }

    /// <summary>
    /// 检查当前拖动距离下的拖动方向
    /// </summary>
    private Direction CheckDirection(Vector2 dragDistance)
    {
        Direction currentDirection;

        if (Mathf.Abs(dragDistance.x) > Mathf.Abs(dragDistance.y))
        {
            if (dragDistance.x > 0)
            {
                currentDirection = Direction.Right;
            }
            else 
            {
                currentDirection = Direction.Left;
            }
        }
        else
        {
            if (dragDistance.y > 0)
            {
                currentDirection = Direction.Up;
            }
            else
            {
                currentDirection = Direction.Down;
            }
        }
        return currentDirection;
    }

}
