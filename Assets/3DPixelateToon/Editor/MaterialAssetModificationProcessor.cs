using UnityEngine;
using UnityEditor;
using System.IO;

// Assings the default shader to all newly created materials
public class MaterialAssetModificationProcessor : AssetModificationProcessor
{
    private static DefaultMaterialSettings settings;
    static void OnWillCreateAsset(string path)
    {
        if (Path.GetExtension(path).ToLower() == ".mat")
        {
            EditorApplication.delayCall += () => ProcessMaterial(path);
        }
    }
    private static void LoadSettings()
    {
        settings = AssetDatabase.LoadAssetAtPath<DefaultMaterialSettings>(PathInfo.DefaultMaterialSettingsPath);
    }

    static void ProcessMaterial(string path)
    {
        LoadSettings();
        if (settings != null)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                material.shader = settings.defaultShader;
                SetMaterialProperties(material, settings);
                EditorUtility.SetDirty(material);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning("Failed to load material at path: " + path);
            }
        }
    }

    private static void SetMaterialProperties(Material material, DefaultMaterialSettings settings)
    {
        if (settings != null)
        {
            material.SetFloat("_Shades", settings.shades);
            material.SetFloat("_Smoothness", settings.smoothness);
            material.SetFloat("_RimThreshold", settings.rimThreshold);
            material.SetFloat("_EdgeDiffuse", settings.edgeDiffuse);
            material.SetFloat("_EdgeSpecular", settings.edgeSpecular);
            material.SetFloat("_EdgeSpecularOffset", settings.edgeSpecularOffset);
            material.SetFloat("_EdgeDistanceAttenuation", settings.edgeDistanceAttenuation);
            material.SetFloat("_EdgeShadowAttenuation", settings.edgeShadowAttenuation);
            material.SetFloat("_EdgeRim", settings.edgeRim);
            material.SetFloat("_EdgeRimOffset", settings.edgeRimOffset);
        }
    }
}
