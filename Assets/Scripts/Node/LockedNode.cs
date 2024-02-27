using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class LockedNode : Node
{
    public List<CipherNode> currentCipherNodes;
    public List<NodeInfo> cipherNodes;
    public List<int> answer;
    private bool isLocked = true;
    private bool hasPopUpCipherNode = false;

    private void OnMouseUp() {
        if (isSelected)
        {
            if (!hasPopUpCipherNode)
            {
                PopUpCipherNodes(cipherNodes);
                hasPopUpCipherNode = true;
            }

            if (isLocked && !hasPopUp && DeLocked())
            {
                DestroyTheCipherNode();
                PopUpChildNode(childNodes);
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
        foreach (CipherNode cipherNode in currentCipherNodes)
        {
            Destroy(cipherNode.transform.gameObject);
        }
    }

    private void PopUpCipherNodes(List<NodeInfo> cipherNodes)
    {
        for (int i = 0; i < cipherNodes.Count; i++)
        {
            Node currentCipherNode = Instantiate(cipherNodes[i].node,transform.position,Quaternion.identity);

            CipherNode cipherNode = currentCipherNode.transform.GetComponent<CipherNode>();
            cipherNode.index = i;
            currentCipherNodes.Add(cipherNode);

            currentCipherNode.transform.DOMove(
                new Vector3(cipherNodes[i].offset.x,cipherNodes[i].offset.y,0),tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentCipherNode.isPoping = true;
                }).OnComplete(() => 
                {
                    currentCipherNode.isPoping = false;
                });
        }
    }

    public bool DeLocked()
    {
        bool corresponed = true;

        for (int i = 0; i < cipherNodes.Count; i++)
        {
            if (answer[i] != currentCipherNodes[i].cipher)
            {
                corresponed = false;
                break;
            }
        }

        Debug.Log(corresponed);
        return corresponed;
    }




}
