

using System;
using System.Collections.Generic;

public class MyScriptableObjectTest : BaseSetting
{
    public List<MyScriptableObjectData> ObjectDataList=new List<MyScriptableObjectData>();
}
[Serializable]
public class MyScriptableObjectData
{
    public string mCName;
    public string mUName;
    public string mPicLoadPath;
    public string mModelLoadPath;
}
