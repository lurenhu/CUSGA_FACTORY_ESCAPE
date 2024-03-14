using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Controllable : MonoBehaviour
{
    [Header("可调整参数")]
    public float maxSpeed = 5f; // 最高速度限制
    public float distance = 2f; // 设置初始长度
    public float frequency = 5f; // 设置弹簧频率
    public float dampingRatio = 0.5f; // 设置阻尼比
    public GameObject hammerPrefab;

    private Rigidbody2D BeControlledRb;
    private Node myNode;
    private SpringJoint2D springJoint;
    private GameObject line;

    private void Start() {
        InitializeReference();
    }

    private void InitializeReference()
    {
        GameObject hammer = Instantiate(hammerPrefab, transform.position, Quaternion.identity, NodeMapBuilder.Instance.transform);

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

    void Update()
    {
        // 限制速度
        LimitVelocity();
    }

    private void OnMouseUp() 
    {
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = true;        
        
        if (myNode.isSelected)
        {
            
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
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
    }
}
