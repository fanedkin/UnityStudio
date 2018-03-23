using UnityEngine;
using System.Collections;
using UnityEditor;

public class PoolManagerEditor
{
    [MenuItem("MyTools/Crate GameObjectPoolConfig")]
    static void CreateGameObjectPoolList()
    {
        GameObjectPoolList poolList = ScriptableObject.CreateInstance<GameObjectPoolList>();
        string path = PoolManager.PoolConfigPath;
        AssetDatabase.CreateAsset(poolList, path);
        AssetDatabase.SaveAssets();
    }
}
