using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AngleLocked : MonoBehaviour
{
    [Header("可调整参数")]
    public GameObject pointerPrefab;
    public float angelRange = 5;
    [Space(5)]
    [Header("观测参数")]
    public List<NodeInfo> pointers;
    public List<float> angles;
    private bool hasPopUpPointers = false;
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();

        LoadPointers();
    }

    /// <summary>
    /// 角度锁密码初始化
    /// </summary>
    public void InitializeAngleLocked(NodeSO nodeSO)
    {
        AngleLockNodeSO angleLockNode = (AngleLockNodeSO)nodeSO;

        angles = angleLockNode.angles;
    }

    /// <summary>
    /// 导入指针节点
    /// </summary>
    public void LoadPointers()
    {
        for (int i = 0; i < angles.Count; i++)
        {
            GameObject pointerGameObject = Instantiate(pointerPrefab, transform.position,Quaternion.identity,transform);

            pointerGameObject.gameObject.SetActive(false);

            Node node = pointerGameObject.GetComponent<Node>();

            node.parentID = myNode.id;

            Vector2 direction = new Vector2(Random.Range(-1,1), Random.Range(-1,1)).normalized;

            NodeInfo pointer = new NodeInfo(){node = node, direction = direction};

            pointers.Add(pointer);
        }
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;
        if (myNode.isDragging) myNode.isDragging = false;

        if (myNode.isSelected)
        {
            if (!hasPopUpPointers)
            {
                myNode.PopUpChildNode(pointers);
                hasPopUpPointers = true;
                return;
            }

            // 节点交互内容
            if (CheckPointerInAngle() && !myNode.hasPopUp)
            {
                ClearAllPointers();
                myNode.PopUpChildNode(myNode.nodeInfos);
                myNode.hasPopUp = true;
            }
            else
            {
                Debug.Log("Not UnLocked");
            }
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);

            myNode.isSelected = true;
        }
    }

    private void ClearAllPointers()
    {
        foreach (NodeInfo nodeInfo in pointers)
        {
            nodeInfo.node.gameObject.SetActive(false);
            LineCreator.Instance.DeleteLine(nodeInfo.node);
        }

        pointers.Clear();
    }

    /// <summary>
    /// 判断所有指针是否都在对应位置
    /// </summary>
    private bool CheckPointerInAngle()
    {
        if (pointers.Count == angles.Count)
        {
            List<float> temp = new List<float>();
            foreach (NodeInfo nodeInfo in pointers)
            {
                float degree = HelperUtility.GetAngleFromVector(nodeInfo.node.transform.localPosition);
                Debug.Log(degree);
                temp.Add(degree);
            }

            return HelperUtility.CheckFloatList(temp, angles, angelRange);
        }
        else
        {
            Debug.Log("指针数量与角度列表数量不匹配");
            return false;
        }
    }
}
