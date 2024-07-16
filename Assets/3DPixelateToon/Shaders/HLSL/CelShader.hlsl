#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

#ifndef SHADERGRAPH_PREVIEW

struct EdgeConstants
{
    float diffuse;
    float specular;
    float specularOffset;
    float distanceAttenuation;
    float shadowAttenuation;
    float rim;
    float rimOffset;
};

struct SurfaceVariables
{
    float3 normal;
    float3 view;
    float smoothness;
    float shininess;
    float rimThreshold;
    EdgeConstants ec;
};

float Toon(float3 normal, float3 lightDir, float Shades)
{
    float cosAngle = max(0.0, dot(normalize(normal), normalize(lightDir)));
    return saturate(floor(cosAngle * Shades) / Shades);
}

float CalculateCelShading(Light l, SurfaceVariables s, float Shades)
{
    float shadowAttenuationSmoothstepped = smoothstep(0.0f, s.ec.shadowAttenuation, l.shadowAttenuation);
    float distanceAttenuationSmoothstepped = smoothstep(0.0f, s.ec.distanceAttenuation, l.distanceAttenuation);
    float attenuation = shadowAttenuationSmoothstepped * distanceAttenuationSmoothstepped;

    float diffuse = Toon(s.normal, l.direction, Shades);
    diffuse *= attenuation;
    diffuse = smoothstep(0.0f, s.ec.diffuse, diffuse);

    float3 h = SafeNormalize(l.direction + s.view);
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *= diffuse * s.smoothness;

    float rim = 1 - Toon(s.view, s.normal, Shades);
    rim *= pow(diffuse, s.rimThreshold);
    
    specular = s.smoothness * smoothstep( (1 - s.smoothness) * s.ec.specular + s.ec.specularOffset, s.ec.specular + s.ec.specularOffset, specular);
    rim = s.smoothness * smoothstep(s.ec.rim - 0.5f * s.ec.rimOffset, s.ec.rim + 0.5f * s.ec.rimOffset, rim);

    return diffuse + max(specular, rim);
}
#endif

void LightingCelShaded_float(float3 Normal, float3 View, float3 Position, float Shades, float Smoothness, float RimThreshold,
                             float EdgeDiffuse, float EdgeSpecular, float EdgeSpecularOffset, float EdgeDistanceAttenuation, float EdgeShadowAttenuation,
                             float EdgeRim, float EdgeRimOffset, out float3 Color)
{
#if defined(SHADERGRAPH_PREVIEW)
    Color = float3(0.5, 0.5, 0.5);
#else
    #if SHADOWS_SCREEN
        float4 clipPos = TransformWorldToHClip(Position);
        float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(Position);
    #endif
    SurfaceVariables s;
    s.normal = Normal;
    s.view = View;
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * s.smoothness + 1);
    s.rimThreshold = RimThreshold;

    s.ec.diffuse = EdgeDiffuse;
    s.ec.specular = EdgeSpecular;
    s.ec.specularOffset = EdgeSpecularOffset;
    s.ec.distanceAttenuation = EdgeDistanceAttenuation;
    s.ec.shadowAttenuation = EdgeShadowAttenuation;
    s.ec.rim = EdgeRim;
    s.ec.rimOffset = EdgeRimOffset;

    Light light = GetMainLight(shadowCoord);
    Color = light.color * CalculateCelShading(light, s, Shades);

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++)
    {
        light = GetAdditionalLight(i, Position, 1);
        Color += light.color * CalculateCelShading(light, s, Shades);
    }
    
#endif
}

#endif