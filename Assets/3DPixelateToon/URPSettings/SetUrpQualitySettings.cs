using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SetURPQualitySettings
{
    // Path to your URP Asset
    public static readonly string scriptPath = GetScriptPath("SetURPQualitySettings");

    public static void Initialize()
    {
        EditorApplication.delayCall += SetURPAssetForAllQualityLevels;
    }

    private static void SetURPAssetForAllQualityLevels()
    {
        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("PathInfo script path could not be determined.");
            return;
        }

        string urpAssetPath = Path.Combine(scriptPath, "URP.asset");
        // Load the URP Asset
        UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpAssetPath);
        if (urpAsset == null)
        {
            Debug.LogError("URP Asset not found at specified path: " + urpAssetPath);
            return;
        }

        // Set the URP Asset for all quality levels
        int qualityLevels = QualitySettings.names.Length;
        for (int i = 0; i < qualityLevels; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = urpAsset;
        }

        // Save the changes
        AssetDatabase.SaveAssets();

        DefaultMaterialAssigner.Initialize();
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
