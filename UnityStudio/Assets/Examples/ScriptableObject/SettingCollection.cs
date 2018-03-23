using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

static class SettingCollection
{
    public static List<BaseSetting> baseSettingCollection = new List<BaseSetting>();


    static SettingCollection()
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        Type[] types = assembly.GetExportedTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(BaseSetting)))
            {
                ScriptableObject scriptableObject=ScriptableObject.CreateInstance(types[i]);
                baseSettingCollection.Add(scriptableObject as BaseSetting);
            }
        }
    }
}