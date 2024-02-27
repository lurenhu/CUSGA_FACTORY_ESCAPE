using UnityEngine;

public class DefaultNode : Node
{

    private void OnMouseUp() {
        if (isSelected)
        {
            Debug.Log(gameObject.name + " has mission");
            
            if (isPoping || hasPopUp) return;

            PopUpChildNode(childNodes);
            hasPopUp = true;
        }
        else
        {
            isSelected = true;
        }
    }

}
