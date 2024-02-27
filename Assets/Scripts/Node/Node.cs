using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class Node : MonoBehaviour
{
    protected Line line;
    public List<NodeInfo> childNodes;// 存储子节点集
    public Node parentNode;// 当前节点的父节点
    [HideInInspector] public bool isPoping = false;// 判断是否处于弹出状态
    [HideInInspector] public bool isDraging = false;
    protected bool isSelected = false;// 判断是否处于被选中状体
    protected bool hasPopUp = false;
    
    [SerializeField] protected float tweenDuring = 2;// 弹出持续时间

    private void Awake() {
        line = GetComponentInChildren<Line>();
    }

    protected virtual void OnMouseDrag() {
        if (!isDraging)
            isDraging = true;
            
        if (!isPoping)
            transform.position = TranslateScreenToWorld(Input.mousePosition);
    }

    /// <summary>
    /// 弹出所有子节点
    /// </summary>
    public void PopUpChildNode(List<NodeInfo> nodes)
    {
        foreach (NodeInfo childNode in nodes)
        {
            Node currentNode = Instantiate(childNode.node,transform.position,Quaternion.identity);

            currentNode.parentNode = this;
            currentNode.transform.GetComponent<Line>().endPoint = currentNode.parentNode.transform;

            currentNode.transform.DOMove(
                new Vector3(childNode.offset.x,childNode.offset.y,0),tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentNode.isPoping = true;
                }).OnComplete(() => 
                {
                    currentNode.isPoping = false;
                });
        }
    }

    /// <summary>
    /// 将屏幕坐标转化为世界坐标
    /// </summary>
    public static Vector3 TranslateScreenToWorld(Vector3 Position)
    {
        Vector3 cameraTranslatePos = Camera.main.ScreenToWorldPoint(Position);
        return new Vector3 (cameraTranslatePos.x,cameraTranslatePos.y,0);
    }

}

[Serializable]
public class NodeInfo
{
    public string text;
    public Node node;
    public Vector2 offset;// 弹出方向偏移距离

}
