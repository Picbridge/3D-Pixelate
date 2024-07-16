using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Assigns the default material to all newly added objects in the scene
public static class DefaultMaterialAssigner
{
    private static DefaultMaterialSettings settings;
    private static Material material;
    private static HashSet<int> existingObjects = new HashSet<int>();

    public static void Initialize()
    {
        EditorApplication.delayCall += InitializeMaterials;
    }
    private static void InitializeMaterials()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.newSceneCreated += OnNewSceneCreated;

        LoadSettings();
        InitializeExistingObjects();
    }

    private static void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
    {
        InitializeExistingObjects();
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        InitializeExistingObjects();
    }

    private static void OnHierarchyChanged()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (!existingObjects.Contains(obj.GetInstanceID()))
            {
                existingObjects.Add(obj.GetInstanceID());
                AssignDefaultMaterialToGameObject(obj);
            }
        }
    }

    private static void LoadSettings()
    {
        settings = AssetDatabase.LoadAssetAtPath<DefaultMaterialSettings>(PathInfo.DefaultMaterialSettingsPath);
    }

    private static void InitializeExistingObjects()
    {
        existingObjects.Clear();

        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            existingObjects.Add(obj.GetInstanceID());
        }
    }

    private static void AssignDefaultMaterialToGameObject(GameObject obj)
    {
        if (obj.TryGetComponent<Renderer>(out Renderer renderer))
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(PathInfo.DefaultMaterialPath);
            if (material == null)
            {
                Debug.LogError("Material not found at path: " + PathInfo.DefaultMaterialPath);
                return;
            }

            renderer.sharedMaterial = material;
        }

        foreach (Transform child in obj.transform)
        {
            AssignDefaultMaterialToGameObject(child.gameObject);
        }
    }

    private static void SetMaterialProperties(Material material, DefaultMaterialSettings settings)
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
