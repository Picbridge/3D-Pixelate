using UnityEngine;
using UnityEditor;

// Custom editor for DefaultMaterialSettings
[CustomEditor(typeof(DefaultMaterialSettings))]
public class DefaultMaterialSettingsEditor : Editor
{
    private DefaultMaterialSettings settings;

    private void OnEnable()
    {
        EditorApplication.update += UpdateMaterialsInEditor;
        if (settings == null)
        {
            settings = (DefaultMaterialSettings)target;
        }

    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateMaterialsInEditor;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Shader Settings", EditorStyles.boldLabel);
        settings.shades = EditorGUILayout.Slider("Shades", settings.shades, 0.0f, 20.0f);

        EditorGUILayout.LabelField("Light Settings", EditorStyles.boldLabel);
        settings.smoothness = EditorGUILayout.Slider("Smoothness", settings.smoothness, 0.0f, 10.0f);
        settings.rimThreshold = EditorGUILayout.Slider("Rim Threshold", settings.rimThreshold, 0.0f, 5.0f);

        EditorGUILayout.LabelField("Edge Settings", EditorStyles.boldLabel);
        settings.edgeDiffuse = EditorGUILayout.Slider("Edge Diffuse", settings.edgeDiffuse, 0.0f, 5.0f);
        settings.edgeSpecular = EditorGUILayout.Slider("Edge Specular", settings.edgeSpecular, 0.0f, 1.0f);
        settings.edgeSpecularOffset = EditorGUILayout.Slider("Edge Specular Offset", settings.edgeSpecularOffset, 0.0f, 1.0f);
        settings.edgeDistanceAttenuation = EditorGUILayout.Slider("Edge Distance Attenuation", settings.edgeDistanceAttenuation, 0.0f, 1.0f);
        settings.edgeShadowAttenuation = EditorGUILayout.Slider("Edge Shadow Attenuation", settings.edgeShadowAttenuation, 0.0f, 5.0f);
        settings.edgeRim = EditorGUILayout.Slider("Edge Rim", settings.edgeRim, 0.0f, 1.0f);
        settings.edgeRimOffset = EditorGUILayout.Slider("Edge Rim Offset", settings.edgeRimOffset, 0.0f, 1.0f);

        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Apply Default Settings"))
        {
            ApplySettingsToAllMaterials();
        }
        EditorUtility.SetDirty(target);
    }

    private void ApplySettingsToAllMaterials()
    {
        if (settings == null || settings.defaultShader == null)
        {
            Debug.LogWarning("Settings or shader is not set.");
            return;
        }

        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null && material.shader == settings.defaultShader)
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
                EditorUtility.SetDirty(material);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Applied default settings to all materials.");
    }

    private void UpdateMaterialsInEditor()
    {
        if (settings == null || settings.defaultShader == null)
            return;

        foreach (string guid in AssetDatabase.FindAssets("t:Material"))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null && material.shader == settings.defaultShader)
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
}
