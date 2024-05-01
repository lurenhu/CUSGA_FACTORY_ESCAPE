using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    [Header("可调整参数")]
    public float maxSpeed = 15f; // 最高速度限制
    public float distance = 2f; // 设置初始长度
    public float frequency = 5f; // 设置弹簧频率
    public float dampingRatio = 0.5f; // 设置阻尼比
    public GameObject hammerPrefab;
    public float Friction = 0.5f;

    private Node myNode;
    private float triggerDistance;
    private Vector2 dragStartPosition;

    private SpringJoint2D springJoint;
    private Rigidbody2D BeControlledRb;
    [HideInInspector] public GameObject line;
    [HideInInspector] public GameObject hammer;

    private void Start() {
        InitializeReference();
    }

    public void InitializeMoving(NodeSO nodeSO)
    {
        MovingNodeSO movingNodeSO= (MovingNodeSO)nodeSO;

        triggerDistance = movingNodeSO.triggerDistance;

        myNode = transform.GetComponent<Node>();
    }

    /// <summary>
    /// 初始化可控制节点相关参数
    /// </summary>
    private void InitializeReference()
    {
        hammer = Instantiate(hammerPrefab, transform.position, Quaternion.identity, NodeMapBuilder.Instance.transform);

        line = LineCreator.Instance.CreateLine(transform, hammer.transform);

        BeControlledRb = hammer.GetComponent<Rigidbody2D>();
        myNode = transform.GetComponentInParent<Node>();
        springJoint = transform.GetComponent<SpringJoint2D>();

        springJoint.autoConfigureDistance = false;
        springJoint.connectedBody = BeControlledRb;
        springJoint.distance = distance; // 设置初始长度
        springJoint.frequency = frequency; // 设置弹簧频率
        springJoint.dampingRatio = dampingRatio; // 设置阻尼比
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

            //如果拖动距离超过阈值，则触发回调函数
            if (dragDistance.magnitude >= triggerDistance)
            {
                StaticEventHandler.CallGetNextNodeLevel();
            }
        }

        LimitVelocity();
    }

    void LimitVelocity()
    {
        Vector2 velocity = BeControlledRb.velocity;
        if (velocity.magnitude > maxSpeed)
        {
            BeControlledRb.velocity = velocity.normalized * maxSpeed;
        }

        if (velocity.magnitude > 0)
        {
            BeControlledRb.velocity -= velocity.normalized * Time.deltaTime * Friction;
        }
    }
}
