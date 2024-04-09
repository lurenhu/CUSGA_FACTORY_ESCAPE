using UnityEngine;

public class TextShow : MonoBehaviour
{

    [Header("观测数据")]    
    public TextAsset text;
    private Node myNode;

    private void Start() {
        myNode = transform.GetComponent<Node>();
    }

    public void InitializeTextNode(NodeSO nodeSO)
    {
        TextNodeSO textNode = (TextNodeSO)nodeSO;

        this.text = textNode.text;
    }

    private void OnMouseUp()
    {
        if (myNode.isPopping) return;

        if (!myNode.isDragging)
        {
            if (myNode.isSelected)
            {
                // 节点交互内容
                UIManager.Instance.DisplayTextNodeContent(text);
            }
            else
            {
                // 删除其他所有节点的选中状态
                NodeMapBuilder.Instance.ClearAllSelectedNode(myNode);
                myNode.GetSelectedAnimate();
    
                myNode.isSelected = true;
                
                // UIManager.Instance.StartDisplayNodeTextForShowRoutine(myNode.nodeTextForShow);
                UIManager.Instance.DisplayNodeText(myNode.nodeTextForShow);
            }

        }
        else
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }
}
