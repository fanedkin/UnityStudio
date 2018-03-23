using UnityEditor;

public class YxSettingsEditor
{
    [MenuItem("Setting File/Creat")]
    static void Creat()
    {
        SettingSelector settingSelector = ScriptableWizard.DisplayWizard<SettingSelector>("Choose Settings");
    }
}