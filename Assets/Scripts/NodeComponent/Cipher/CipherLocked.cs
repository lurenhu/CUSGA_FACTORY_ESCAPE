using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CipherLocked : MonoBehaviour
{
    [Header("可调整参数")]
    public GameObject cipherPrefab;

    [Header("观测参数")]
    public List<int> cipherValues;
    public List<NodeInfo> cipherNodes;

    private bool hasPopUpCipherNode = false;// 判断节点是否弹出密码节点
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();

        LoadCipherNodes();
    }

    public void InitializeCipherLocked(NodeSO nodeSO)
    {
        LockedNodeSO lockedNode = (LockedNodeSO)nodeSO;

        cipherValues = lockedNode.cipherValues;
    }

    /// <summary>
    /// 初始化所有锁节点
    /// </summary>
    private void LoadCipherNodes()
    {
        for (int i = 0; i < cipherValues.Count; i++)
        {
            GameObject currentCipherNode = Instantiate(cipherPrefab, transform.position, Quaternion.identity,transform);

            Cipher cipher = currentCipherNode.GetComponent<Cipher>();

            cipher.InitializeCipherNode(i,0);

            cipher.myNode.parentID = myNode.id;

            LineCreator.Instance.CreateLine(cipher.myNode);

            currentCipherNode.SetActive(false);

            Vector2 direction = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized;

            NodeInfo tempNodeInfo = new NodeInfo(){node = cipher.myNode, direction = direction};

            cipherNodes.Add(tempNodeInfo);
        }
    }

    private void OnMouseUp() 
    {
        if (myNode.isPopping) return;
        
        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 第一次点击弹出所有的锁节点
                if (!hasPopUpCipherNode)
                {
                    myNode.PopUpChildNode(cipherNodes);
                    hasPopUpCipherNode = true;
                    return;
                }
    
                // 第二次点击判断是否解锁然后弹出所有子节点
                if (UnLocked() && !myNode.hasPopUp)
                {
                    DestroyAllCipherNode();
                    myNode.PopUpChildNode(myNode.nodeInfos);
                    myNode.hasPopUp = true;
                }
    
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }
        }
        
        if (myNode.isDragging) myNode.isDragging = true;        
    }

    /// <summary>
    /// 删除所有锁节点
    /// </summary>
    private void DestroyAllCipherNode()
    {
        foreach (NodeInfo nodeInfo in cipherNodes)
        {
            nodeInfo.node.gameObject.SetActive(false);
            LineCreator.Instance.DeleteLine(nodeInfo.node);
        }

        cipherNodes.Clear();
    }

    /// <summary>
    /// 判断是否当前节点是否解锁
    /// </summary>
    private bool UnLocked()
    {
        if (cipherNodes.Count == cipherValues.Count)
        {
            for (int i = 0; i < cipherNodes.Count; i++)
            {
                Cipher cipherNode = cipherNodes[i].node.GetComponent<Cipher>();

                if (cipherNode.value != cipherValues[i])
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
