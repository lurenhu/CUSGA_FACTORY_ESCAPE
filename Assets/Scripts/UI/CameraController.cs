using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 lastMousePos;
    //public GameObject BG;
    
    private void Update()
    {
        // 获取鼠标位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 发射一条射线从摄像机到鼠标位置
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // 如果射线与游戏对象碰撞，执行相应的操作
        if (hit.collider == null && !GameManager.Instance.haveNodeDrag)
        {
            // 当用户按下鼠标左键时
            if (Input.GetMouseButtonDown(0))
            {
                // 记录当前鼠标位置
                lastMousePos = mousePosition;
            }
            // 当用户持续拖动鼠标时
            else if (Input.GetMouseButton(0))
            {
                // 计算鼠标移动的距离
                Vector2 delta = lastMousePos - mousePosition;

                // 将摄像机移动与鼠标移动相反的方向
                Camera.main.transform.Translate(delta);
            }
        }

        //BG.transform.position = new Vector3(transform.position.x,transform.position.y,0);
    }
}
