using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform startPoint;
    [HideInInspector] public Transform endPoint;
    public float ropeWidth = 0.1f;
    public float ropeResolution = 0.5f;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        startPoint = transform.parent;
    }

    private void Update()
    {
        if (endPoint != null)
        {
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
            }
            DrawRope();
        }
    }

    private void DrawRope()
    {
        int numPoints = Mathf.CeilToInt(Vector3.Distance(startPoint.position, endPoint.position) * ropeResolution);
        lineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            Vector3 pointPosition = Vector3.Lerp(startPoint.position, endPoint.position, t);
            lineRenderer.SetPosition(i, pointPosition);
        }
    }
    
}
