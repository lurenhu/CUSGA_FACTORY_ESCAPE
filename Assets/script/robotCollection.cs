using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class robotCollection
{
    public string name;
    public string botid; 
    public robotCollection(string name, string botid)
    {
        this.name = name;
        this.botid = botid;
    }

}
