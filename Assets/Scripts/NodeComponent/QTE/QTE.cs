using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QTE : MonoBehaviour
{
    [Header("观测数据")]
    public Node myNode;
    public float dragDistanceThreshold = 2;
    private Vector2 dragStartPosition;
    private Direction direction;
    private SpriteRenderer spriteRenderer;

    private void Start() {
        switch (direction)
        {
            case Direction.Left:
                spriteRenderer.sprite = GameResources.Instance.QTESprites.Find(x => x.direction == Direction.Left).sprite;
                break;
            case Direction.Right:
                spriteRenderer.sprite = GameResources.Instance.QTESprites.Find(x => x.direction == Direction.Right).sprite;
                break;
            case Direction.Up:
                spriteRenderer.sprite = GameResources.Instance.QTESprites.Find(x => x.direction == Direction.Up).sprite;
                break;
            case Direction.Down:
                spriteRenderer.sprite = GameResources.Instance.QTESprites.Find(x => x.direction == Direction.Down).sprite;
                break;
            default:
                Debug.Log("No This Direction");
                return;
        }

    }

    /// <summary>
    /// 传递QTE数据
    /// </summary>
    public void InitializeQTE(NodeSO nodeSO)
    {
        QTENodeSO qTENodeSO = (QTENodeSO)nodeSO;

        dragDistanceThreshold = qTENodeSO.dragDistance;

        direction = qTENodeSO.direction;

        myNode = transform.GetComponent<Node>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

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
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
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
            Vector2 dragCurrentPosition = Input.mousePosition;

            Vector2 dragDistance = dragCurrentPosition - dragStartPosition;

            Direction currentDirection = CheckDirection(dragDistance);

            //如果拖动距离超过阈值，则触发回调函数
            if (dragDistance.magnitude >= dragDistanceThreshold)
            {
                if (currentDirection == direction)
                {
                    StartCoroutine(myNode.PopUpChildNode(myNode.nodeInfos));
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
