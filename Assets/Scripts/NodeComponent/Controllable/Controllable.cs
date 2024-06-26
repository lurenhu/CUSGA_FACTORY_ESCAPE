using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Controllable : MonoBehaviour
{
    [Header("可调整参数")]
    public float maxSpeed = 15f; // 最高速度限制
    public float distance = 2f; // 设置初始长度
    public float frequency = 5f; // 设置弹簧频率
    public float dampingRatio = 0.5f; // 设置阻尼比
    public GameObject hammerPrefab;
    public float Friction = 0.5f;

    private Rigidbody2D BeControlledRb;
    [SerializeField] private Node myNode;
    private SpringJoint2D springJoint;
    [HideInInspector] public GameObject line;
    [HideInInspector] public GameObject hammer;
    private string targetNodeID;
    private float speedToPop;
    private bool hasSetSpeed = false;

    private void Start() {
        InitializeReference();
    }

    /// <summary>
    /// 传递编辑器参数至可控制节点
    /// </summary>
    public void InitializeControllable(NodeSO node)
    {
        ControllableNodeSO controllableNode = (ControllableNodeSO)node;

        targetNodeID = controllableNode.collidedNodeId;
        speedToPop = controllableNode.speedToPop;

    }

    /// <summary>
    /// 初始化可控制节点相关参数
    /// </summary>
    private void InitializeReference()
    {
        hammer = Instantiate(hammerPrefab, transform.position + new Vector3(1,1,1), Quaternion.identity, NodeMapBuilder.Instance.transform);

        line = LineCreator.Instance.CreateLine(transform, hammer.transform);

        BeControlledRb = hammer.GetComponent<Rigidbody2D>();
        myNode = transform.GetComponent<Node>();
        springJoint = transform.GetComponent<SpringJoint2D>();

        springJoint.autoConfigureDistance = false;
        springJoint.connectedBody = BeControlledRb;
        springJoint.distance = distance; // 设置初始长度
        springJoint.frequency = frequency; // 设置弹簧频率
        springJoint.dampingRatio = dampingRatio; // 设置阻尼比
    }

    void Update()
    {
        // 限制速度
        LimitVelocity();

        // 给目标节点添加速度检测
        if (!hasSetSpeed)
            AddSpeedDetectorToTargetNode();
    }


    private void OnMouseUp() 
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;
        
        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
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

    // 限制速度
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

    
    private void AddSpeedDetectorToTargetNode()
    {
        NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(targetNodeID,out Node targetNode);
        if (targetNode != null)
        {
            targetNode.transform.GetComponent<BoxCollider2D>().isTrigger = false;

            SpeedDetector speedDetector = targetNode.gameObject.AddComponent<SpeedDetector>();
            speedDetector.SetSpeedToPop(speedToPop);

            hasSetSpeed = true;
        }
        else
        {
            Debug.Log("目标节点不存在");
        }
    }
}
