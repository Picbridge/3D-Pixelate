using System.IO;
using UnityEditor;
using UnityEngine;

public static class PathInfo
{
    public static readonly string scriptPath = GetScriptPath("AssetLoader");

    public static string DefaultMaterialSettingsPath
    {
        get
        {
            if (string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogError("PathInfo script path could not be determined.");
                return null;
            }

            string settingsPath = Path.Combine(scriptPath, "Editor/Settings/DefaultMaterialSettings.asset");
            return settingsPath;
        }
    }

    public static string DefaultMaterialPath
    {
        get
        {
            if (string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogError("PathInfo script path could not be determined.");
                return null;
            }
            string materialPathPath = Path.Combine(scriptPath, "Editor/Settings/DefaultMaterial.mat");
            return materialPathPath;
        }
    }

    private static string GetScriptPath(string scriptName)
    {
        string[] guids = AssetDatabase.FindAssets($"{scriptName} t:Script");
        if (guids.Length == 0)
        {
            Debug.LogError($"{scriptName} script not found!");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return Path.GetDirectoryName(path);
    }
}
