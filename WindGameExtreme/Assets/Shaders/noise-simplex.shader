// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Noise-Simplex" {
Properties {
	_Freq ("Frequency", Float) = 1
	_Speed ("Speed", Float) = 1
}

SubShader {
	Pass {
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members noiseVal)
#pragma exclude_renderers d3d11
		
		#pragma target 3.0
		
		#pragma vertex vert
		#pragma fragment frag
		
		#include "noiseSimplex.cginc"
		

		
		ENDCG
	}
}

}