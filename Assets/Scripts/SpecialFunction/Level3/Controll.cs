using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controll : MonoBehaviour
{
    [Header("可调整参数")]
    public float maxSpeed = 15f; // 最高速度限制
    public float distance = 2f; // 设置初始长度
    public float frequency = 5f; // 设置弹簧频率
    public float dampingRatio = 0.5f; // 设置阻尼比
    public GameObject hammerPrefab;
    private Rigidbody2D BeControlledRb;
    private SpringJoint2D springJoint;
    [HideInInspector] public GameObject line;
    [HideInInspector] public GameObject hammer;

    private bool hasSetSpeed = false;

    private Node myNode;

    private string targetNodeID;

    private void Start() {
        InitializeReference();
    }

    void Update()
    {
        // 给目标节点添加速度检测
        if (!hasSetSpeed && !myNode.isPopping)
            AddSpeedDetectorToTargetNode();
    }

    public void InitializeControl(NodeSO nodeSO)
    {
        ControllNodeSO controllNodeSO= (ControllNodeSO)nodeSO;

        targetNodeID = controllNodeSO.targetNodeID;

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

    private void AddSpeedDetectorToTargetNode()
    {
        NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(targetNodeID,out Node targetNode);
        if (targetNode != null)
        {
            targetNode.transform.GetComponent<BoxCollider2D>().isTrigger = false;

            targetNode.gameObject.AddComponent<CollisionTrigger>();

            hasSetSpeed = true;
        }
        else
        {
            Debug.Log("目标节点不存在");
        }
    }
}
