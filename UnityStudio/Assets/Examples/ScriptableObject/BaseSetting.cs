using System;
using UnityEditor;
using UnityEngine;

[Serializable]
[SettingAttribute]
public class BaseSetting : ScriptableObject
{
    /// <summary>  
    /// save file.  
    /// </summary>  
    public void Save()
    {
        EditorUtility.SetDirty(this);
    }
}