using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    public sealed class ScriptableRenderer
    {
        // When there is no support to StruturedBuffer lights data is setup in a constants data
        // we also limit the amount of lights that can be shaded per object to simplify shading
        // in these low end platforms (GLES 2.0 and GLES 3.0)

        // Amount of Lights that can be shaded per object (in the for loop in the shader)
        // This uses unity_4LightIndices0 to store 4 per-object light indices
        const int k_MaxPerObjectAdditionalLightsNoStructuredBuffer = 4;

        // Light data is stored in a constant buffer (uniform array)
        // This value has to match MAX_VISIBLE_LIGHTS in Input.hlsl
        const int k_MaxVisibleAdditionalLightsNoStructuredBuffer = 16;

        // Point and Spot Lights are stored in a StructuredBuffer.
        // We shade the amount of light per-object as requested in the pipeline asset and
        // we can store a great deal of lights in our global light buffer
        const int k_MaxVisibleAdditioanlLightsStructuredBuffer = 4096;

        public static bool useStructuredBufferForLights
        {
            get
            {
                // TODO: Graphics Emulation are breaking StructuredBuffers disabling it for now until
                // we have a fix for it
                return false;
                // return SystemInfo.supportsComputeShaders &&
                //        SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore &&
                //        !Application.isMobilePlatform &&
                //        Application.platform != RuntimePlatform.WebGLPlayer;
            }
        }

        public int maxPerObjectAdditionalLights
        {
            get
            {
                return useStructuredBufferForLights ?
                    8 : k_MaxPerObjectAdditionalLightsNoStructuredBuffer;
            }
        }

        public int maxVisibleAdditionalLights
        {
            get
            {
                return useStructuredBufferForLights ?
                    k_MaxVisibleAdditioanlLightsStructuredBuffer :
                    k_MaxVisibleAdditionalLightsNoStructuredBuffer;
            }
        }

        public PostProcessRenderContext postProcessingContext { get; private set; }

        public ComputeBuffer perObjectLightIndices { get; private set; }

        static Mesh s_FullscreenMesh = null;
        static Mesh fullscreenMesh
        {
            get
            {
                if (s_FullscreenMesh != null)
                    return s_FullscreenMesh;

                float topV = 1.0f;
                float bottomV = 0.0f;

                s_FullscreenMesh = new Mesh { name = "Fullscreen Quad" };
                s_FullscreenMesh.SetVertices(new List<Vector3>
                {
                    new Vector3(-1.0f, -1.0f, 0.0f),
                    new Vector3(-1.0f,  1.0f, 0.0f),
                    new Vector3(1.0f, -1.0f, 0.0f),
                    new Vector3(1.0f,  1.0f, 0.0f)
                });

                s_FullscreenMesh.SetUVs(0, new List<Vector2>
                {
                    new Vector2(0.0f, bottomV),
                    new Vector2(0.0f, topV),
                    new Vector2(1.0f, bottomV),
                    new Vector2(1.0f, topV)
                });

                s_FullscreenMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);
                s_FullscreenMesh.UploadMeshData(true);
                return s_FullscreenMesh;
            }
        }

        List<ScriptableRenderPass> m_ActiveRenderPassQueue = new List<ScriptableRenderPass>();

        List<ShaderPassName> m_LegacyShaderPassNames = new List<ShaderPassName>()
        {
            new ShaderPassName("Always"),
            new ShaderPassName("ForwardBase"),
            new ShaderPassName("PrepassBase"),
            new ShaderPassName("Vertex"),
            new ShaderPassName("VertexLMRGBM"),
            new ShaderPassName("VertexLM"),
        };

        const string k_ReleaseResourcesTag = "Release Resources";
        readonly Material[] m_Materials;

        public ScriptableRenderer(LightweightRenderPipelineAsset pipelineAsset)
        {
            if (pipelineAsset == null)
                throw new ArgumentNullException("pipelineAsset");

            m_Materials = new[]
            {
                CoreUtils.CreateEngineMaterial("Hidden/InternalErrorShader"),
                CoreUtils.CreateEngineMaterial(pipelineAsset.copyDepthShader),
                CoreUtils.CreateEngineMaterial(pipelineAsset.samplingShader),
                CoreUtils.CreateEngineMaterial(pipelineAsset.blitShader),
                CoreUtils.CreateEngineMaterial(pipelineAsset.screenSpaceShadowShader),
            };

            postProcessingContext = new PostProcessRenderContext();
        }

        public void Dispose()
        {
            if (perObjectLightIndices != null)
            {
                perObjectLightIndices.Release();
                perObjectLightIndices = null;
            }

            for (int i = 0; i < m_Materials.Length; ++i)
                CoreUtils.Destroy(m_Materials[i]);
        }

        public void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Keywords are enabled while executing passes.
            CommandBuffer cmd = CommandBufferPool.Get("Clear Pipeline Keywords");
            cmd.DisableShaderKeyword(ShaderKeywordStrings.MainLightShadows);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.MainLightShadowCascades);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.AdditionalLightsVertex);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.AdditionalLightsPixel);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.AdditionalLightShadows);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.SoftShadows);
            cmd.DisableShaderKeyword(ShaderKeywordStrings.MixedLightingSubtractive);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            for (int i = 0; i < m_ActiveRenderPassQueue.Count; ++i)
                m_ActiveRenderPassQueue[i].Execute(this, context, ref renderingData);

            DisposePasses(ref context);
        }

        public Material GetMaterial(MaterialHandle handle)
        {
            int handleID = (int)handle;
            if (handleID >= m_Materials.Length)
            {
                Debug.LogError(string.Format("Material {0} is not registered.",
                    Enum.GetName(typeof(MaterialHandle), handleID)));
                return null;
            }

            return m_Materials[handleID];
        }

        public void Clear()
        {
            m_ActiveRenderPassQueue.Clear();
        }

        public void EnqueuePass(ScriptableRenderPass pass)
        {
            m_ActiveRenderPassQueue.Add(pass);
        }

        public void SetupPerObjectLightIndices(ref CullResults cullResults, ref LightData lightData)
        {
            if (lightData.additionalLightsCount == 0)
                return;

            List<VisibleLight> visibleLights = lightData.visibleLights;
            int[] perObjectLightIndexMap = cullResults.GetLightIndexMap();

            int directionalLightsCount = 0;
            int additionalLightsCount = 0;    

            // Disable all directional lights from the perobject light indices
            // Pipeline handles them globally.
            for (int i = 0; i < visibleLights.Count; ++i)
            {
                if (additionalLightsCount >= maxVisibleAdditionalLights)
                    break;

                VisibleLight light = visibleLights[i];
                if (light.lightType == LightType.Directional)
                {
                    perObjectLightIndexMap[i] = -1;
                    ++directionalLightsCount;
                }
                else
                {
                    perObjectLightIndexMap[i] -= directionalLightsCount;
                    ++additionalLightsCount;
                }
            }

            // Disable all remaining lights we cannot fit into the global light buffer.
            for (int i = directionalLightsCount + additionalLightsCount; i < visibleLights.Count; ++i)
                perObjectLightIndexMap[i] = -1;

            cullResults.SetLightIndexMap(perObjectLightIndexMap);

            // if not using a compute buffer, engine will set indices in 2 vec4 constants
            // unity_4LightIndices0 and unity_4LightIndices1
            if (useStructuredBufferForLights)
            {
                int lightIndicesCount = cullResults.GetLightIndicesCount();
                if (lightIndicesCount > 0)
                {
                    if (perObjectLightIndices == null)
                    {
                        perObjectLightIndices = new ComputeBuffer(lightIndicesCount, sizeof(int));
                    }
                    else if (perObjectLightIndices.count < lightIndicesCount)
                    {
                        perObjectLightIndices.Release();
                        perObjectLightIndices = new ComputeBuffer(lightIndicesCount, sizeof(int));
                    }

                    cullResults.FillLightIndices(perObjectLightIndices);
                }
            }
        }

        public void RenderPostProcess(CommandBuffer cmd, ref CameraData cameraData, RenderTextureFormat colorFormat, RenderTargetIdentifier source, RenderTargetIdentifier dest, bool opaqueOnly)
        {
            Camera camera = cameraData.camera;
            postProcessingContext.Reset();
            postProcessingContext.camera = camera;
            postProcessingContext.source = source;
            postProcessingContext.sourceFormat = colorFormat;
            postProcessingContext.destination = dest;
            postProcessingContext.command = cmd;
            postProcessingContext.flip = !cameraData.isStereoEnabled && camera.targetTexture == null;

            if (opaqueOnly)
                cameraData.postProcessLayer.RenderOpaqueOnly(postProcessingContext);
            else
                cameraData.postProcessLayer.Render(postProcessingContext);
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        public void RenderObjectsWithError(ScriptableRenderContext context, ref CullResults cullResults, Camera camera, FilterRenderersSettings filterSettings, SortFlags sortFlags)
        {
            Material errorMaterial = GetMaterial(MaterialHandle.Error);
            if (errorMaterial != null)
            {
                DrawRendererSettings errorSettings = new DrawRendererSettings(camera, m_LegacyShaderPassNames[0]);
                for (int i = 1; i < m_LegacyShaderPassNames.Count; ++i)
                    errorSettings.SetShaderPassName(i, m_LegacyShaderPassNames[i]);

                errorSettings.sorting.flags = sortFlags;
                errorSettings.rendererConfiguration = RendererConfiguration.None;
                errorSettings.SetOverrideMaterial(errorMaterial, 0);
                context.DrawRenderers(cullResults.visibleRenderers, ref errorSettings, filterSettings);
            }
        }

        public static RenderTextureDescriptor CreateRenderTextureDescriptor(ref CameraData cameraData, float scaler = 1.0f)
        {
            Camera camera = cameraData.camera;
            RenderTextureDescriptor desc;
            float renderScale = cameraData.renderScale;

            if (cameraData.isStereoEnabled)
            {
                return XRGraphics.eyeTextureDesc;
            }
            else
            {
                desc = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight);
            }
            desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR :
                RenderTextureFormat.Default;
            desc.enableRandomWrite = false;
            desc.sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            desc.width = (int)((float)desc.width * renderScale * scaler);
            desc.height = (int)((float)desc.height * renderScale * scaler);
            return desc;
        }

        public static bool RequiresIntermediateColorTexture(ref CameraData cameraData, RenderTextureDescriptor baseDescriptor)
        {
            if (cameraData.isOffscreenRender)
                return false;

            bool isScaledRender = !Mathf.Approximately(cameraData.renderScale, 1.0f);
            bool isTargetTexture2DArray = baseDescriptor.dimension == TextureDimension.Tex2DArray;
            bool noAutoResolveMsaa = cameraData.msaaSamples > 1 && !SystemInfo.supportsMultisampleAutoResolve;
            return noAutoResolveMsaa || cameraData.isSceneViewCamera || isScaledRender || cameraData.isHdrEnabled ||
                   cameraData.postProcessEnabled || cameraData.requiresOpaqueTexture || isTargetTexture2DArray || !cameraData.isDefaultViewport;
        }

        public static ClearFlag GetCameraClearFlag(Camera camera)
        {
            if (camera == null)
                throw new ArgumentNullException("camera");

            // LWRP doesn't support CameraClearFlags.DepthOnly.
            // In case of skybox we know all pixels will be rendered to screen so
            // we don't clear color. In Vulkan/Metal this becomes DontCare load action
            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
                return ClearFlag.Depth;

            // Otherwise we clear color + depth. This becomes either a clear load action or glInvalidateBuffer call
            // on mobile devices. On PC/Desktop a clear is performed by blitting a full screen quad.
            return ClearFlag.All;
        }

        public static RendererConfiguration GetRendererConfiguration(int additionalLightsCount)
        {
            RendererConfiguration configuration = RendererConfiguration.PerObjectReflectionProbes | RendererConfiguration.PerObjectLightmaps | RendererConfiguration.PerObjectLightProbe;
            if (additionalLightsCount > 0)
            {
                if (useStructuredBufferForLights)
                    configuration |= RendererConfiguration.ProvideLightIndices;
                else
                    configuration |= RendererConfiguration.PerObjectLightIndices8;
            }

            return configuration;
        }

        public static void RenderFullscreenQuad(CommandBuffer cmd, Material material, MaterialPropertyBlock properties = null, int shaderPassId = 0)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            cmd.DrawMesh(fullscreenMesh, Matrix4x4.identity, material, 0, shaderPassId, properties);
        }

        public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material material)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            // TODO: In order to issue a copyTexture we need to also check if source and dest have same size
            //if (SystemInfo.copyTextureSupport != CopyTextureSupport.None)
            //    cmd.CopyTexture(source, dest);
            //else
            cmd.Blit(source, dest, material);
        }

        void DisposePasses(ref ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get(k_ReleaseResourcesTag);

            for (int i = 0; i < m_ActiveRenderPassQueue.Count; ++i)
                m_ActiveRenderPassQueue[i].FrameCleanup(cmd);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
