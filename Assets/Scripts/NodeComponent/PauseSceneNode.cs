using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PauseSceneNode : MonoBehaviour
{
    private Node myNode;

    public UnityEvent OnTrigger;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping || UIManager.Instance.UIShow) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
    
                // 弹出子节点
                if (!myNode.hasPopUp && myNode.nodeInfos.Count != 0)
                {
                    StartCoroutine(PopUpChildNode(myNode.nodeInfos));
                    myNode.hasPopUp = true;
                    return;
                }

                if (OnTrigger != null)
                {
                    OnTrigger.Invoke();
                }

            }
            else
            {
                // 删除其他所有节点的选中状态
                GamePause.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
            }

            // 播放音频
            if (myNode.audios.Count != 0)
            {
                soundManager.Instance.PlayMusic(myNode.audios[0]);
            }
            // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
            // UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }


    private IEnumerator PopUpChildNode(List<NodeInfo> nodeInfos)
    {
        foreach (NodeInfo childNode in nodeInfos)
        {
            Node currentNode = childNode.node; // Instantiate(childNode.node,transform.position,Quaternion.identity);

            currentNode.transform.position = transform.position;
            currentNode.gameObject.SetActive(true);

            GamePause.Instance.CreateLine(currentNode);
            soundManager.Instance.PlaySFX("NodeBorn");

            soundManager.Instance.PlaySFX("NodeBorn");
            Sequence sequence = DOTween.Sequence();
            sequence.Append(currentNode.transform.DOMove(
                childNode.direction * GameManager.Instance.popUpForce,GameManager.Instance.tweenDuring
                ).SetRelative().OnStart(() => 
                {
                    currentNode.isPopping = true;
                }).OnComplete(() => 
                {
                    currentNode.isPopping = false;
                }));
                
            sequence.Append(currentNode.transform.DOScale(new Vector3(1f,0.3f,1),0.1f));
            sequence.Append(currentNode.transform.DOScale(new Vector3(1f,1f,1),0.1f));

            yield return new WaitForSeconds(0.1f);
        }
    }
}
