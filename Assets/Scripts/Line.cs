using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    // private const int NODENUMBER = 2;
    // private LineRenderer lineRenderer;

    // private void Awake() {
    //     lineRenderer = GetComponent<LineRenderer>();
    // }

    // private void Start() {
    //     lineRenderer.positionCount = NODENUMBER;
    // }
    
    // public void SetPosition(Vector2 startPosition,Vector2 endPosition)
    // {
    //     lineRenderer.SetPosition(0,startPosition);
    //     lineRenderer.SetPosition(1,endPosition);
    // }

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

        startPoint = transform;
    }

    private void Update()
    {
        if (endPoint == null) return;

        DrawRope();
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
