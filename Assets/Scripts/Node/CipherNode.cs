using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CipherNode : Node
{
    public int cipher;
    public int index;

    private void OnMouseUp() {
        if (cipher == 9)
            cipher = 0;
        else
            cipher++;
    }
}
