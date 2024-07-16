using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class OutlinePass : ScriptableRenderPass
{

    private readonly Material outlineMaterial;

    private RenderTargetIdentifier cameraColorTarget;

    private RenderTargetIdentifier temporaryBuffer;

    private int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");
    public OutlinePass(RenderPassEvent renderPassEvent, OutlineFeature.OutlineSettings settings)
    {
        this.renderPassEvent = renderPassEvent;

        outlineMaterial = new Material(Shader.Find("Hidden/Outline"));
        if (!outlineMaterial)
            return;
        //outlineMaterial = CoreUtils.CreateEngineMaterial("Hidden/Outline");
        outlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);
        outlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
        outlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);
        outlineMaterial.SetVector("_NormalEdgeBias", settings.normalEdgeBias);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
#pragma warning restore CS0618 // Type or member is obsolete
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;
        cmd.GetTemporaryRT(temporaryBufferID, descriptor, FilterMode.Bilinear);
        temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!outlineMaterial)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Outlines")))
        {

#pragma warning disable CS0618 // Type or member is obsolete
            Blit(cmd, cameraColorTarget, temporaryBuffer);
            Blit(cmd, temporaryBuffer, cameraColorTarget, outlineMaterial);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryBufferID);
    }

}