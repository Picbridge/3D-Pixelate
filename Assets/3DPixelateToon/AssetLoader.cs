using UnityEngine;
using UnityEditor;

public static class AssetLoader
{
    private static bool isInitialized = false;

    [InitializeOnLoadMethod]
    private static void LoadAssets()
    {
        EditorApplication.update += WaitForAssetsToLoad;
    }

    private static void WaitForAssetsToLoad()
    {
        if (isInitialized)
        {
            return;
        }

        // Load necessary assets here
        string settingsPath = PathInfo.DefaultMaterialSettingsPath;
        DefaultMaterialSettings settings = AssetDatabase.LoadAssetAtPath<DefaultMaterialSettings>(settingsPath);
        bool allAssetsLoaded = true;

        // Check Pixelize shader
        Shader pixelizeShader = Shader.Find("Hidden/Pixelize");
        if (pixelizeShader == null)
        {
            Debug.LogError("Pixelize.shader could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("Pixelize.shader loaded successfully.");
        }

        // Check Cel shader
        Shader celShader = Shader.Find("Shader Graphs/CelShader");
        if (pixelizeShader == null)
        {
            Debug.LogError("CelShader.shadergraph could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("CelShader.shadergraph loaded successfully.");
        }

        // Check Outline shader
        Shader outlineShader = Shader.Find("Hidden/Outline");
        if (pixelizeShader == null)
        {
            Debug.LogError("Outline.shadergraph could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("Outline.shadergraph loaded successfully.");
        }

        // Check UnlitColor shader graph
        var unlitColorShaderGraph = Shader.Find("Hidden/UnlitColor");
        if (unlitColorShaderGraph == null)
        {
            Debug.LogError("UnlitColor.shadergraph could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("UnlitColor.shadergraph loaded successfully.");
        }

        // Check ViewSpaceNormals shader graph
        var viewSpaceNormalsShaderGraph = Shader.Find("Hidden/ViewSpaceNormals");
        if (viewSpaceNormalsShaderGraph == null)
        {
            Debug.LogError("ViewSpaceNormals.shadergraph could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("ViewSpaceNormals.shadergraph loaded successfully.");
        }

        // Check if DefaultMaterialSettings is loaded
        if (settings == null)
        {
            Debug.LogError("DefaultMaterialSettings.asset could not be loaded.");
            allAssetsLoaded = false;
        }
        else
        {
            Debug.Log("DefaultMaterialSettings loaded successfully.");
        }

        if (allAssetsLoaded)
        {
            Debug.Log("All necessary assets loaded successfully.");

            // Trigger the next initialization step
            SetURPQualitySettings.Initialize();

            isInitialized = true;
            EditorApplication.update -= WaitForAssetsToLoad; // Stop checking
        }
    }
}
