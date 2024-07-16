using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ViewSpaceNormalsTexturePass : ScriptableRenderPass
{

    private OutlineFeature.ViewSpaceNormalsTextureSettings normalsTextureSettings;
    private FilteringSettings filteringSettings;
    private FilteringSettings occluderFilteringSettings;

    private readonly List<ShaderTagId> shaderTagIdList;
    private readonly Material normalsMaterial;
    private readonly Material occludersMaterial;

#pragma warning disable CS0618 // Type or member is obsolete
    private readonly RenderTargetHandle normals;
#pragma warning restore CS0618 // Type or member is obsolete

    public ViewSpaceNormalsTexturePass(RenderPassEvent renderPassEvent, LayerMask layerMask, LayerMask occluderLayerMask, OutlineFeature.ViewSpaceNormalsTextureSettings settings)
    {
        this.renderPassEvent = renderPassEvent;
        this.normalsTextureSettings = settings;
        filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
        occluderFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, occluderLayerMask);

        shaderTagIdList = new List<ShaderTagId> {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        normals.Init("_SceneViewSpaceNormals");
        normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
        if (!normalsMaterial)
            return;
        //normalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/ViewSpaceNormals");
        occludersMaterial = new Material(Shader.Find("Hidden/UnlitColor"));
        if (!occludersMaterial)
            return;
        //occludersMaterial = CoreUtils.CreateEngineMaterial("Hidden/UnlitColor");

        occludersMaterial.SetColor("_Color", normalsTextureSettings.backgroundColor);
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
        normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
        normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
        cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, normalsTextureSettings.filterMode);
#pragma warning disable CS0618 // Type or member is obsolete
        ConfigureTarget(normals.Identifier());
#pragma warning restore CS0618 // Type or member is obsolete

        ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!normalsMaterial || !occludersMaterial)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.perObjectData = normalsTextureSettings.perObjectData;
            drawSettings.enableDynamicBatching = normalsTextureSettings.enableDynamicBatching;
            drawSettings.enableInstancing = normalsTextureSettings.enableInstancing;
            drawSettings.overrideMaterial = normalsMaterial;

            DrawingSettings occluderSettings = drawSettings;
            occluderSettings.overrideMaterial = occludersMaterial;

            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
            context.DrawRenderers(renderingData.cullResults, ref occluderSettings, ref occluderFilteringSettings);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(normals.id);
    }

}
