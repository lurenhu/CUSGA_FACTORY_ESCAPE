using System.Collections.Generic;
using UnityEngine;

public class LockedNode : Node
{
    [Space(10)]
    [Header("LOCK NODE PROPERTISE")]
    [Tooltip("该节点的所有锁节点")]
    public List<NodeInfo> cipherNodes;
    [Tooltip("该锁节点的解锁值")]
    public Dictionary<Node,int> cipherDictionary;
    private bool isLocked = true;
    private bool hasPopUpCipherNode = false;

    protected override void Awake()
    {
        base.Awake();

        LoadCipherNodes();

        LoadCipherDictionary();
        
    }

    private void LoadCipherNodes()
    {
        foreach (Transform child in transform)
        {
            CipherNode cipherNode = child.GetComponent<CipherNode>();

            Vector2 direction = new Vector2(Random.Range(-1,1),Random.Range(-1,1)).normalized;

            NodeInfo nodeInfo = new NodeInfo()
            {
                node = cipherNode,
                direction = direction
            };

            cipherNodes.Add(nodeInfo);
        }
    }

    private void LoadCipherDictionary()
    {
        foreach (NodeInfo cipherNode in cipherNodes)
        {
            CipherNode node = cipherNode.node as CipherNode;

            int i = 1;

            cipherDictionary.Add(node,i);
        }
    }

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            if (!hasPopUpCipherNode)
            {
                PopUpChildNode(cipherNodes);
                hasPopUpCipherNode = true;
            }

            if (isLocked && !hasPopUp && DeLocked())
            {
                DestroyTheCipherNode();
                PopUpChildNode(nodeInfos);
                hasPopUp = true;
                isLocked = false;
            }
        }
        else
        {
            isSelected = true;
        }
    }

    private void DestroyTheCipherNode()
    {
        foreach (NodeInfo cipherNode in cipherNodes)
        {
            Destroy(cipherNode.node.transform.gameObject);
        }
    }

    // private void PopUpCipherNodes(List<NodeInfo> cipherNodes)
    // {
    //     for (int i = 0; i < cipherNodes.Count; i++)
    //     {
    //         Node currentCipherNode = Instantiate(cipherNodes[i].node,transform.position,Quaternion.identity);

    //         CipherNode cipherNode = currentCipherNode.transform.GetComponent<CipherNode>();
    //         cipherNode.index = i;
    //         currentCipherNodes.Add(cipherNode);

    //         currentCipherNode.transform.DOMove(
    //             new Vector3(cipherNodes[i].offset.x,cipherNodes[i].offset.y,0),tweenDuring
    //             ).SetRelative().OnStart(() => 
    //             {
    //                 currentCipherNode.isPoping = true;
    //             }).OnComplete(() => 
    //             {
    //                 currentCipherNode.isPoping = false;
    //             });
    //     }
    // }

    public bool DeLocked()
    {
        foreach (NodeInfo cipherNode in cipherNodes)
        {
            CipherNode currentCipherNode = cipherNode.node as CipherNode;

            cipherDictionary.TryGetValue(currentCipherNode,out int answer);

            if (currentCipherNode.cipher != answer)
            {
                return false;
            }
        }
        return true;
    }




}
