using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeNode : Node
{
    [Space(10)]
    [Header("PROBE NODE")]
    private SpriteRenderer indicatorLight;
    public float blinkSpeed = 0.5f; // 闪烁速度
    public Color targetColor = Color.green; // 目标颜色
    private Color originalColor; // 初始颜色

    public Transform target; // 目标位置
    public float detectionDistance = 10f; // 检测距离
    public float interactiveDistance = 5f; // 交互距离

    private bool isBlinking = false;

    protected override void Start()
    {
        base.Start();

        target = NodeMapBuilder.Instance.nodeHasCreated.TryGetValue(nodeProperty.targetIDForDetection,out Node targetNode)? targetNode.transform:null;

        indicatorLight = GetComponent<SpriteRenderer>();

        originalColor = indicatorLight.color;
    }

    void Update()
    {
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
            }
        }
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
