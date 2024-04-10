using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class SaveProfile<T> where T : SaveProfileData
{
    public string profileName;
    public T saveData;

    private SaveProfile() { }

    public SaveProfile(string name, T saveData)
    {
        this.profileName = name;
        this.saveData = saveData;
    }
}

public abstract class SaveProfileData {}
