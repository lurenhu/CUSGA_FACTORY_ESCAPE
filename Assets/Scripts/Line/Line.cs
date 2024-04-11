using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform startPoint;
    public Transform endPoint;
    public float ropeWidth = 0.1f;
    public float ropeResolution = 0.5f;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;
    }

    private void Update()
    {
        if (startPoint == null || endPoint == null)
        {
            return;
        }
        DrawRope();
    }

    /// <summary>
    /// 绘制线条
    /// </summary>
    private void DrawRope()
    {
        int numPoints = 2;// Mathf.CeilToInt(Vector2.Distance(startPoint.position, endPoint.position) * ropeResolution);
        lineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 pointPosition = Vector3.Lerp(startPoint.position, endPoint.position, t);
            lineRenderer.SetPosition(i, pointPosition);
        }
    }

    /// <summary>
    /// 节点数据初始化
    /// </summary>
    public void InitializeLine(Transform startPoint, Transform endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
    
}
