// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/aa"
{
	Properties
	{

		_Frequency("Frequency", float) = 1
		_Speed("Speed", float) = 1

		}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

			CGPROGRAM
			#pragma surface surf Lambert vertex:vert
			//#pragma vertex vert
			//#pragma fragment frag

			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"

			struct v2f
			{
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
				
			};

			struct Input {
				float3 normal;
			};

			uniform float _Frequency;
			uniform float _Speed;

			void vert (inout appdata_full v)
			{
				
			}

			void surf (Input IN, inout SurfaceOutput o) {
				
				o.Albedo = 1;
				o.Vert = 1;
			}
			ENDCG
		
	}
}
