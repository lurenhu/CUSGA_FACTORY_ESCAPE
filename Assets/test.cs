using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float dragDistanceThreshold = 5f; // 定义拖动的距离阈值
    public Vector2 dragDirection = Vector2.right; // 定义拖动的方向

    private Vector3 dragStartPosition;
    private bool isDragging = false;

    // Update is called once per frame
    void Update()
    {
        // 鼠标左键按下开始拖动
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse Down");
            dragStartPosition = Input.mousePosition;
            isDragging = true;
        }

        // 鼠标左键持续按下时进行拖动判断
        if (isDragging && Input.GetMouseButton(0))
        {
            Debug.Log("MouseDrag");

            Vector3 dragCurrentPosition = Input.mousePosition;
            float dragDistance = Vector3.Dot(dragCurrentPosition - dragStartPosition, dragDirection);
            Debug.Log(dragDistance);
            // 如果拖动距离超过阈值，则触发回调函数
            if (dragDistance >= dragDistanceThreshold)
            {
                Debug.Log(1);
                isDragging = false; // 重置拖动状态
            }
        }
         
    }
}
