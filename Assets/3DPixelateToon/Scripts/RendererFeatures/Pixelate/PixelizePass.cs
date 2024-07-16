using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PixelizePass : ScriptableRenderPass
{
    private readonly Material pixelizeMaterial;
    private PixelizeFeature.PixelizePassSettings settings;

    private RenderTargetIdentifier colorBuffer, pixelBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");
    private int pixelScreenHeight, pixelScreenWidth;

    public PixelizePass(RenderPassEvent renderPassEvent, PixelizeFeature.PixelizePassSettings settings)
    {
        this.renderPassEvent = renderPassEvent;
        //pixelizeMaterial = new Material(Shader.Find("Hidden/Pixelize"));
        //if (!pixelizeMaterial)
        //    return;
        pixelizeMaterial = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
        if (pixelizeMaterial == null) return;
        this.settings = settings;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
#pragma warning restore CS0618 // Type or member is obsolete
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        pixelScreenHeight = settings.screenHeight;
        pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

        pixelizeMaterial.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        pixelizeMaterial.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        pixelizeMaterial.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        descriptor.height = pixelScreenHeight;
        descriptor.width = pixelScreenWidth;

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("Pixelize Pass")))
        {

#pragma warning disable CS0618 // Type or member is obsolete
            Blit(cmd, colorBuffer, pixelBuffer, pixelizeMaterial);
            Blit(cmd, pixelBuffer, colorBuffer);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
        //cmd.ReleaseTemporaryRT(pointBufferID);
    }

}