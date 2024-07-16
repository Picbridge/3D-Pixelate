using UnityEngine;

// Default material settings
[CreateAssetMenu(fileName = "DefaultMaterialSettings", menuName = "Custom/Material/DefaultMaterialSettings")]
public class DefaultMaterialSettings : ScriptableObject
{
    public Shader defaultShader;
    public float shades = 3.0f;
    public float smoothness = 0.1f;
    public float rimThreshold = 0.5f;
    public float edgeDiffuse = 1.0f;
    public float edgeSpecular = 1.0f;
    public float edgeSpecularOffset = 0.0f;
    public float edgeDistanceAttenuation = 1.0f;
    public float edgeShadowAttenuation = 1.0f;
    public float edgeRim = 1.0f;
    public float edgeRimOffset = 0.0f;

    private void OnEnable()
    {
        if (defaultShader == null)
        {
            defaultShader = Shader.Find("Shader Graphs/CelShader");
        }
    }
}
