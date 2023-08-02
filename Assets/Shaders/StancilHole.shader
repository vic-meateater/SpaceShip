Shader "Custom/StencilHole"
{
	Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TextureOffset("Hole Radius", Range(0, 0.9)) = 0.2
    }

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry-1"
		"ForceNoShadowCasting" = "True" }
		LOD 200
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#pragma surface surf NoLighting alpha Lambert vertex:vert

		sampler2D _MainTex;

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		struct Input
		{
			float2 uv_MainTex;
		};

        fixed4 _Color;

		float _TextureOffset;

		void vert (inout appdata_full v) 
		{
          v.vertex.xz += v.normal.xz * _TextureOffset;
		  v.vertex.y = 0;
		}


		void surf(Input IN, inout SurfaceOutput o)
		{
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}