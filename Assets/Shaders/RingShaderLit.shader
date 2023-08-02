Shader "Lit/RingShader"
  {
      Properties
      {
        _BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        _AlphaScale ("Alpha Scale", Range(0, 2)) = 1

		_Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5

		[Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0

		_TextureOffset("Ring Radius", Range(0, 0.9)) = 0.2
        _RotationSpeed("RotationSpeed", Range(0, 3)) = 0.5
      }
      SubShader
     {
         Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
         Cull Off

         Stencil
		 {
		     Ref 1
		     Comp NotEqual
		 }

         Pass
         {
             ZWrite On
             ColorMask 0
         }

         Pass
         {
			Tags {
				"LightMode" = "CustomLit"
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite Off

            HLSLPROGRAM
			#pragma target 3.5
			#pragma shader_feature _CLIPPING
			#pragma shader_feature _PREMULTIPLY_ALPHA
			#pragma multi_compile_instancing
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			#include "../ShaderLibrary/LitPassRing.hlsl"

			ENDHLSL
 
           //  CGPROGRAM
           //  #pragma vertex vert
           //  #pragma fragment frag
             
           //  #include "UnityCG.cginc"
           //  #include "Lighting.cginc"

 
           //  struct a2v
           //  {
           //      float4 vertex : POSITION;
           //      float3 normal : NORMAL;
           //      float4 texcoord : TEXCOORD0;
           //  };
 
           //  struct v2f
           //  {
           //      float4 pos : SV_POSITION;
           //      float2 uv : TEXCOORD0;
           //      float3 worldNormal : TEXCOORD1;
           //      float3 worldPos : TEXCOORD2;
           //  };
 
           //  sampler2D _MainTex;
           //  float4 _MainTex_ST;
           //  fixed4 _Color;
           //  fixed _AlphaScale;
           //  float _TextureOffset;
           //  float _RingRotationSpeed;
             
           //  v2f vert (a2v v)
           //  {
           //      v2f o;
 
           //      v.vertex.xz += v.normal.xz * _TextureOffset;
		         //v.vertex.y = 0;
           //      o.pos = UnityObjectToClipPos(v.vertex);

           //      v.texcoord.x += _RingRotationSpeed * _Time;
           //      o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
           //      o.worldNormal = UnityObjectToWorldNormal(v.normal);
           //      o.worldPos = mul(unity_ObjectToWorld, v.vertex);
 
           //      return o;
           //  }
             
           //  fixed4 frag (v2f i) : SV_Target
           //  {
           //      fixed3 worldNormal = normalize(i.worldNormal);
           //      fixed3 worldPos = normalize(i.worldPos);
           //      fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
           //      fixed4 texColor = tex2D(_MainTex, i.uv);
           //      fixed3 albedo =  texColor.rgb * _Color.rgb;
           //      fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo;
           //      fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
           //      return fixed4(ambient + diffuse, texColor.a * _AlphaScale);
           //  }
           //  ENDCG
         }
     }
 }
