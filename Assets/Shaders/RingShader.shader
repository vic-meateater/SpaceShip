Shader "Custom/RingShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
		_TextureOffset("Ring Radius", Range(0, 0.9)) = 0.2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		Tags { "RenderType"="Opaque" }

		 
		LOD 200
		Stencil
		{
			Ref 1
			Comp NotEqual
		}
				

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows BlinnPhong alpha Lambert vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		sampler2D _MainTex;
		float _TextureOffset;

		void vert (inout appdata_full v) 
		{
          v.vertex.xz += v.normal.xz * _TextureOffset;
		  v.vertex.y = 0;

		  v.texcoord.x += 2 * _Time;
		}

		float4 _Vertex;
		struct Input
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed _AlphaScale;
		fixed4 _Color;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			c *= _Color;
			c.a *= _AlphaScale;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
