using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Probe : MonoBehaviour
{
    private string targetNodeID;
    private Node myNode;

    private SpriteRenderer indicatorLight;
    public float blinkSpeed = 0.5f; // 闪烁速度
    public Color targetColor = Color.green; // 目标颜色
    private Color originalColor; // 初始颜色

    public Transform target; // 目标位置
    public float detectionDistance = 10f; // 检测距离
    public float interactiveDistance = 3f; // 交互距离

    private bool isBlinking = false;

    private void Start() {
        myNode = transform.GetComponent<Node>();

        indicatorLight = GetComponentInChildren<SpriteRenderer>();

        originalColor = indicatorLight.color;
    }

    public void InitializeProbe(NodeSO nodeSO)
    {
        ProbeNodeSO probeNode = (ProbeNodeSO)nodeSO;

        targetNodeID = probeNode.targetIDForDetection;
    }
    
    void Update()
    {
        if (NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(targetNodeID, out Node targetNode) && target == null && !myNode.isPopping)
        {
            target = targetNode.transform;
        }

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < detectionDistance && distance > interactiveDistance) 
            {
                StartBlink();
                blinkSpeed = Mathf.Lerp(0.1f, 1f, (distance - interactiveDistance)/(detectionDistance - interactiveDistance));
            }
            else if (distance < interactiveDistance)
            {
                StopBlink();
                target.gameObject.SetActive(true);
            }
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

    // 开始闪烁
    void StartBlink()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkRoutine());
        }
    }

    // 停止闪烁
    void StopBlink()
    {
        if (isBlinking)
        {
            StopCoroutine(BlinkRoutine());
            indicatorLight.color = targetColor;
        }
    }

    IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        while (true)
        {
            // 切换指示灯的颜色
            indicatorLight.color = targetColor;
            yield return new WaitForSeconds(blinkSpeed);
            indicatorLight.color = originalColor;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
