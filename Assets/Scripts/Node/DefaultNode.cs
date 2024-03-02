using UnityEngine;

public class DefaultNode : Node
{

    protected override void OnMouseUp() {
        base.OnMouseUp();
        
        if (isSelected)
        {
            Debug.Log(gameObject.name + " has mission");
            
            if (hasPopUp) return;
            PopUpChildNode(nodeInfos);
            hasPopUp = true;
        }
        else
        {
            isSelected = true;
        }
    }

}
