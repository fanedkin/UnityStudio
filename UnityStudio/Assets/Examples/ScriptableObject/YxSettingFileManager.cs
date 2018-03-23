using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// <summary>  
/// management of setting files  
/// </summary>  
public static class YxSettingFileManager
{
    internal const string settingPath = "Assets/Resources/Settings/{0}.asset";

    public static bool isCreating { get; set; }

    public static SettingFileResult Creat(string fileName)
    {
        isCreating = true;

        //文件路径  
        var realPath = PathName(fileName);

        //目录路径  
        var path = Path.GetDirectoryName(realPath);

        //对应目录是否存在  
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.ImportAsset(path);
        }

        //从配置文件列表中取出对应类  
        BaseSetting fileObj = Get(fileName);

        if (fileObj == null)
            return SettingFileResult.ERROR;

        //对应文件是否存在  
        if (File.Exists(realPath))
            return SettingFileResult.ALREADYHAVE;

        //创建  
        AssetDatabase.CreateAsset(fileObj, realPath);
        return SettingFileResult.SUCCESS;
    }

    static string PathName(string fileName)
    {
        return string.Format(settingPath, fileName);
    }

    /// <summary>  
    /// 通过配置文件名获取对应类  
    /// </summary>  
    public static BaseSetting Get(string settingName)
    {
        var mList = SettingCollection.baseSettingCollection;
        for (int i = 0; i < mList.Count; ++i)
        {
            if (string.CompareOrdinal(mList[i].GetType().Name, settingName) == 0)
                return mList[i];
        }
        return null;
    }

    /// <summary>  
    /// 获取配置文件类名集合  
    /// </summary>  
    public static List<string> Keys()
    {
        return SettingCollection.baseSettingCollection.ConvertAll<string>(m => m.GetType().Name);
    }

    public static BaseSetting LoadAsset<T>() where T : BaseSetting
    {
        return AssetDatabase.LoadAssetAtPath<T>(PathName(typeof(T).Name));
    }
}