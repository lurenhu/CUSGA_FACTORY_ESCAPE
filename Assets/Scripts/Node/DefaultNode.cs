using UnityEngine;

public class DefaultNode : Node
{

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            // 节点交互内容

            if (hasPopUp) return;
            PopUpChildNode(nodeInfos);
            hasPopUp = true;
        }
        else
        {
            // 删除其他所有节点的选中状态
            NodeMapBuilder.Instance.ClearAllSelectedNode(this);

            isSelected = true;
        }
    }

}
