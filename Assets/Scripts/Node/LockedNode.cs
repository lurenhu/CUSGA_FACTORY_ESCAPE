using System.Collections.Generic;
using UnityEngine;

public class LockedNode : Node
{
    [Space(10)]
    [Header("LOCK NODE PROPERTISE")]
    [Tooltip("该节点的所有锁节点")]
    public List<NodeInfo> cipherNodes;
    private GameObject cipherPrefab;
    private bool isLocked = true;// 判断节点是否解锁
    private bool hasPopUpCipherNode = false;// 判断节点是否弹出密码节点

    protected override void Start()
    {
        base.Start();

        cipherPrefab = GameResources.Instance.cipherPrefab;
        LoadCipherNodes();
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            // 第一次点击弹出所有的锁节点
            if (!hasPopUpCipherNode)
            {
                PopUpChildNode(cipherNodes);
                hasPopUpCipherNode = true;
                return;
            }

            // 第二次点击判断是否解锁然后弹出所有子节点
            if (UnLocked() && isLocked && !hasPopUp)
            {
                DestroyAllCipherNode();
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

    /// <summary>
    /// 初始化所有锁节点
    /// </summary>
    private void LoadCipherNodes()
    {
        for (int i = 0; i < nodeProperty.cipherNumber; i++)
        {
            GameObject currentCipherNode = Instantiate(cipherPrefab, transform.position, Quaternion.identity,transform);

            CipherNode cipherNode = currentCipherNode.GetComponent<CipherNode>();

            cipherNode.InitializeCipherNode(i,0);

            cipherNode.nodeProperty.parentID = this.id;

            currentCipherNode.SetActive(false);

            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            NodeInfo tempNodeInfo = new NodeInfo(){node = cipherNode, direction = direction};

            cipherNodes.Add(tempNodeInfo);
        }
    }

    /// <summary>
    /// 删除所有锁节点
    /// </summary>
    private void DestroyAllCipherNode()
    {
        for (int i = 0; i < cipherNodes.Count; i++)
        {
            LineCreator.Instance.DeleteLine(cipherNodes[i].node);
            Destroy(cipherNodes[i].node.gameObject);
        }

        cipherNodes.Clear();
    }

    /// <summary>
    /// 判断是否当前节点是否解锁
    /// </summary>
    private bool UnLocked()
    {
        if (cipherNodes.Count == nodeProperty.cipherValues.Count)
        {
            for (int i = 0; i < cipherNodes.Count; i++)
            {
                CipherNode cipherNode = cipherNodes[i].node as CipherNode;

                if (cipherNode.value != nodeProperty.cipherValues[i])
                {
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("锁节点数量与值列表数量不匹配");
            return false;
        }

        return true;
    }





}
