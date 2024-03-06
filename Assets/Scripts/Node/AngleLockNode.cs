using System.Collections.Generic;
using UnityEngine;

public class AngleLockNode : Node
{  
    [Space(10)]
    [Header("ANGLE LOCK NODE")]
    public List<NodeInfo> pointers;
    public GameObject pointerPrefab;
    public bool hasPopUpPointers = false;
    private const float angelRange = 5;

    protected override void Start()
    {
        base.Start();

        LoadPointers();
    }

    /// <summary>
    /// 导入指针
    /// </summary>
    private void LoadPointers()
    {
        for (int i = 0; i < nodeProperty.angles.Count; i++)
        {
            GameObject pointerGameObject = Instantiate(pointerPrefab, transform.position,Quaternion.identity,transform);

            pointerGameObject.gameObject.SetActive(false);

            Node node = pointerGameObject.GetComponent<Node>();

            node.nodeProperty.parentID = this.id;

            Vector2 direction = new Vector2(Random.Range(-1,1), Random.Range(-1,1)).normalized;

            NodeInfo pointer = new NodeInfo(){node = node, direction = direction};

            pointers.Add(pointer);
        }
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

        if (isSelected)
        {
            if (!hasPopUpPointers)
            {
                PopUpChildNode(pointers);
                hasPopUpPointers = true;
                return;
            }

            // 节点交互内容
            if (CheckPointerInAngle() && !hasPopUp)
            {
                ClearAllPointers();
                PopUpChildNode(nodeInfos);
                hasPopUp = true;
            }
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }

    private void ClearAllPointers()
    {
        foreach (NodeInfo nodeInfo in pointers)
        {
            LineCreator.Instance.DeleteLine(nodeInfo.node);
            Destroy(nodeInfo.node.gameObject);
        }

        pointers.Clear();
    }

    private bool CheckPointerInAngle()
    {
        if (pointers.Count == nodeProperty.angles.Count)
        {
            for (int i = 0; i < pointers.Count; i++)
            {
                float degree = HelperUtility.GetAngleFromVector(pointers[i].node.transform.localPosition);
                if (degree < nodeProperty.angles[i] + angelRange && degree > nodeProperty.angles[i] - angelRange)
                    return true;
                else
                    return false;
            }
        }
        else
        {
            Debug.Log("指针数量与角度列表数量不匹配");
            return false;
        }

        return true;
    }
}
