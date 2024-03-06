using UnityEngine;
using UnityEngine.UI;

public class GraphNode : Node
{
    [Space(10)]
    [Header("GRAPH NODE")]
    private Transform graphTransform;

    private void Awake() {
        graphTransform = GameManager.Instance.graphTransform;
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

        if (isSelected)
        {
            // 节点交互内容
            PopUpGraph();
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }

    private void PopUpGraph()
    {
        Image image = graphTransform.Find("Image").GetComponent<Image>();

        image.sprite = nodeProperty.image;

        graphTransform.gameObject.SetActive(true);
    }
}
