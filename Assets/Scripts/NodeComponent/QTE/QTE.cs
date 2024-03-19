using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class QTE : MonoBehaviour
{
    [Header("观测数据")]
    public Direction direction;
    private Direction currentDirection;
    private Node myNode;
    private Rigidbody2D rb;

    private void Start() {
        myNode = transform.GetComponent<Node>();

        rb = transform.GetComponent<Rigidbody2D>();
    }

    public void InitializeQTE(NodeSO nodeSO)
    {
        QTENodeSO qTENodeSO = (QTENodeSO)nodeSO;

        direction = qTENodeSO.direction;
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
        if (myNode.isDragging || myNode.isPopping) return;

        CheckCurrentDirection();
    }

    private void CheckCurrentDirection() 
    {
        Vector2 moveDirection = rb.velocity.normalized;
        Debug.Log("moveDirection: " + moveDirection);

        while (moveDirection != Vector2.zero)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
            {
                if (moveDirection.x > 0)
                {
                    currentDirection = Direction.Right;
                }
                else if (moveDirection.x < 0)
                {
                    currentDirection = Direction.Left;
                }
            }
            else if (Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
            {
                if (moveDirection.y > 0)
                {
                    currentDirection = Direction.Up;
                }
                else if (moveDirection.y < 0)
                {
                    currentDirection = Direction.Down;
                }
            }
        }

        if (currentDirection == direction && myNode.hasPopUp)
        {
            myNode.PopUpChildNode(myNode.nodeInfos);

            myNode.hasPopUp = true;
        }
    }

}
