using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Input", "Lighting", "Light Info")]
public class MainLightNode : CodeFunctionNode
{
    public override bool hasPreview { get { return false; } }

    private static string functionBodyForReals = @"{
			Light mainLight = GetMainLight();
			Color = mainLight.color;
			Direction = mainLight.direction;
			float4 shadowCoord;
			#ifdef LIGHTWEIGHT_SHADOWS_INCLUDED
			#if SHADOWS_SCREEN
				float4 clipPos = TransformWorldToHClip(WorldPos);
				shadowCoord = ComputeScreenPos(clipPos);
			#else
				shadowCoord = TransformWorldToShadowCoord(WorldPos);
			#endif
			#endif
				Attenuation = MainLightRealtimeShadow(shadowCoord);
		}";
    private static string functionBodyPreview = @"{
			Color = 1;
			Direction = float3(-0.5, .5, 0.5);
			Attenuation = 1;
		}";

    private static bool isPreview;

    private static string functionBody
    {
        get
        {
            if (isPreview)
                return functionBodyPreview;
            else
                return functionBodyForReals;
        }
    }


    public MainLightNode()
    {
        name = "Light";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("CustomFunction", BindingFlags.Static | BindingFlags.NonPublic);
    }


    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }

    private static string CustomFunction(
    [Slot(0, Binding.None)] out Vector3 Direction,
    [Slot(1, Binding.None)] out Vector1 Attenuation,
    [Slot(2, Binding.None)] out Vector3 Color,
    [Slot(3, Binding.WorldSpacePosition)] Vector3 WorldPos)
    {
        Direction = Vector3.zero;
        Color = Vector3.zero;

        return functionBody;
    }
}