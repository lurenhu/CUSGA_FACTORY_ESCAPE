using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableNode : Node
{
    [Space(10)]
    [Header("CONTROLLABLE NODE")]
    public float maxSpeed = 5f; // 最高速度限制

    public float distance = 2f; // 设置初始长度
    public float frequency = 5f; // 设置弹簧频率
    public float dampingRatio = 0.5f; // 设置阻尼比

    public GameObject BeControlledObjectPrefab;
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;

    override protected void Start() {
        base.Start();

        InitializePrefab();
    }

    private void InitializePrefab()
    {
        GameObject BeControlledObject = Instantiate(BeControlledObjectPrefab, transform.position, Quaternion.identity,NodeMapBuilder.Instance.transform);

        LineCreator.Instance.CreateLine(transform, BeControlledObject.transform);

        rb = BeControlledObject.GetComponent<Rigidbody2D>();

        springJoint = gameObject.GetComponent<SpringJoint2D>();
        springJoint.autoConfigureDistance = false;
        springJoint.connectedBody = rb;
        springJoint.distance = distance; // 设置初始长度
        springJoint.frequency = frequency; // 设置弹簧频率
        springJoint.dampingRatio = dampingRatio; // 设置阻尼比
    }

    void Update()
    {
        // 限制速度
        LimitVelocity();
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            // 节点交互内容

        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }   

    // 限制速度
    void LimitVelocity()
    {
        Vector2 velocity = rb.velocity;
        if (velocity.magnitude > maxSpeed)
        {
            rb.velocity = velocity.normalized * maxSpeed;
        }
    }
}
