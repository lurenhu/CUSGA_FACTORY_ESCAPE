using UnityEngine;

public class DefaultNode : Node
{

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            if (hasPopUp) return;
            PopUpChildNode(nodeInfos);
            hasPopUp = true;
        }
    }

}
