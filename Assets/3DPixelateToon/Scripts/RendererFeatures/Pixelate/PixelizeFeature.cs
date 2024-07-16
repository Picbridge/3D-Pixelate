using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PixelizeFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelizePassSettings
    {
        [Header("Pixelize Settings")]
        public int screenHeight = 144;
    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

    [SerializeField] private PixelizePassSettings pixelizeSettings = new PixelizePassSettings();

    private PixelizePass pixelizePass;

    public override void Create()
    {
        pixelizePass = new PixelizePass(renderPassEvent, pixelizeSettings);
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(pixelizePass);
    }
}