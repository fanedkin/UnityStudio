using System;
using UnityEditor;
using UnityEngine;

public enum SettingFileResult
{
    ERROR,
    ALREADYHAVE,
    SUCCESS
}
public class SettingSelector : ScriptableWizard
{
    string mStr = "";
    Vector2 mScrollPosition;

    private void OnGUI()
    {
        var keys = YxSettingFileManager.Keys();
        GUILayout.Label("Creat settings", "TL Selection H2");
        DrawSeparator();
        //搜索框
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(84f);
        string before = mStr;
        GUI.SetNextControlName("SearchTextField");
        string after = EditorGUILayout.TextField("", before, "SearchTextField");
        if (after != before) mStr = after;

        if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
        {
            mStr = "";
            GUIUtility.keyboardControl = 0;
        }

        if (Event.current.Equals(Event.KeyboardEvent("return")))//按下回车键就自动对焦输入框
        {
            if (GUI.GetNameOfFocusedControl() != "SearchTextField")
                GUI.FocusControl("SearchTextField");
        }

        GUILayout.Space(84f);
        EditorGUILayout.EndHorizontal();
        //搜索框
        mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);

        string[] filters = mStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < keys.Count; ++i)
        {
            bool isMatch = true;
            for (int j = 0; j < filters.Length; ++j)
            {
                if (keys[i].IndexOf(filters[j], StringComparison.OrdinalIgnoreCase) == -1)
                {
                    isMatch = false;
                    break;
                }
            }

            if (!isMatch)
                continue;

            EditorGUILayout.BeginHorizontal();
            bool select = GUILayout.Button(keys[i], EditorStyles.textField);
            select |= GUILayout.Button("Creat", GUILayout.Width(120f));
            if (select && !YxSettingFileManager.isCreating)
            {
                SettingFileResult result = YxSettingFileManager.Creat(keys[i]);
                switch (result)
                {
                    case SettingFileResult.ALREADYHAVE:
                        EditorUtility.DisplayDialog("ALREADY HAVE", string.Format("Already created a setting file with name {0}", keys[i]), "OK");
                        break;
                    case SettingFileResult.SUCCESS:
                        Close();
                        break;
                    default:
                        break;
                }
            }

            if (YxSettingFileManager.isCreating)
                YxSettingFileManager.isCreating = false;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }


    // use NGUI editors.  
    private void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }
}