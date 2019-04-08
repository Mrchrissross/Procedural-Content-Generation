Shader "Custom/sail-shader"
{
	Properties
	{
		[Header(Appearance properties)]
		_Color("Color", Color) = (1,1,1,1)
		_TintColor("Tint color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		// _NoiseTex ("Noise Texture (RGB)", 2D) = "white" {}
		[KeywordEnum(Normal, Toon)] _ToonShading("Shading mode", Int) = 1
		_Bands("Toon bands", Int) = 5

		[Header(Deformation properties)]
		_VertFrequency("Vertex sine frequency", Range(0, 150)) = 0.1
		_Frequency("Frequency", Range(0, 150)) = 0.1
		_Magnitude("Magnitude", Range(0, 150)) = 0.1
		_Direction("Direction vector", Vector) = (0, 1, 0, 0)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			Pass
			{
				Tags {"LightMode" = "ForwardBase"}

				CGPROGRAM

				#pragma fragment frag
				#pragma vertex vert
				#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"

				#pragma target 3.0

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;

				uniform sampler2D _NoiseTex;
				uniform float4 _NoiseTex_ST;

				uniform fixed4 _Color;
				uniform fixed4 _TintColor;
				uniform float _VertFrequency;
				uniform bool _ToonShading;
				uniform int _Bands;

				uniform float _Frequency;
				uniform float _Magnitude;
				uniform half3 _Direction;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float2 uv2 : TEXCOORD1;
					float4 vertex : SV_POSITION;
					float3 normal : NORMAL;
					float3 oldPos : NORMAL1;
				};

				float3 deform(float3 input, float2 uv)
				{

					float d = dot(normalize(_Direction), input);
					float a = (d * _VertFrequency) + (_Time * _Frequency);
					float m = abs(sin(a)) * _Magnitude * tex2Dlod(_NoiseTex, float4(uv,0,0));

					return m;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.oldPos = v.vertex.xyz;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.uv, _NoiseTex);
					o.vertex = UnityObjectToClipPos(v.vertex + deform(v.vertex.xyz, o.uv2));
					o.normal = UnityObjectToWorldNormal(v.normal);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float3 d = deform(i.oldPos, i.uv2);

					half nl = max(0, dot(d, _WorldSpaceLightPos0.xyz));

					if (_ToonShading)
						nl = round(nl * _Bands) / _Bands;

					float4 diff = nl * _LightColor0;

					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv) * diff * _Color + (_TintColor * _TintColor.a);
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);

					//col = round(col * 5.0) / 5.0;

					return col;
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}