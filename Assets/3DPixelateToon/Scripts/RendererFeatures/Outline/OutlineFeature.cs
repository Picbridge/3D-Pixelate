using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class OutlineFeature : ScriptableRendererFeature {

    [System.Serializable]
    public class OutlineSettings {

        [Header("General Outline Settings")]
        [Range(0.0f, 10.0f)]
        public float outlineScale = 1.0f;
        
        [Header("Depth Settings")]
        [Range(0.0f, 2.0f)]
        public float depthThreshold = 0.5f;

        [Header("Normal Settings")]
        [Range(0.0f, 3.0f)]
        public float normalThreshold = 0.4f;
        public Vector3 normalEdgeBias = new Vector3(1, 1, 1);
    }

    [System.Serializable]
    public class ViewSpaceNormalsTextureSettings {

        [Header("General Scene View Space Normal Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits = 16;
        public FilterMode filterMode;
        public Color backgroundColor = Color.black;

        [Header("View Space Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;

    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    [SerializeField] private LayerMask outlinesLayerMask;
    [SerializeField] private LayerMask outlinesOccluderLayerMask;
    
    [SerializeField] private OutlineSettings outlineSettings = new OutlineSettings();
    [SerializeField] private ViewSpaceNormalsTextureSettings viewSpaceNormalsTextureSettings = new ViewSpaceNormalsTextureSettings();

    private ViewSpaceNormalsTexturePass viewSpaceNormalsTexturePass;
    private OutlinePass outlinePass;
    
    public override void Create() {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(renderPassEvent, outlinesLayerMask, outlinesOccluderLayerMask, viewSpaceNormalsTextureSettings);
        outlinePass = new OutlinePass(renderPassEvent, outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(viewSpaceNormalsTexturePass);
        renderer.EnqueuePass(outlinePass);
    }

}
