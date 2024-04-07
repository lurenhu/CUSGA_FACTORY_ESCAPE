using UnityEngine;

public class TextShow : MonoBehaviour
{

    [Header("观测数据")]    
    public string text;
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
            }

            StartCoroutine(UIManager.Instance.DisplayNodeTextForShow(myNode.nodeTextForShow));
        }
        
        if (myNode.isDragging)
        {
            myNode.isDragging = false;
            GameManager.Instance.haveNodeDrag = false;
        } 
    }
}
