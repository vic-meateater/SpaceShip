Shader "Unlit/StancilHole"
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
         Cull Off

         Stencil
         {
             Ref 1
             Comp Always
             Pass Replace
         }

         Pass
         {
            Tags {"LightMode" = "CustomLit"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             
             #include "UnityCG.cginc"
             #include "Lighting.cginc"
 
             struct a2v
             {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
                 float4 texcoord : TEXCOORD0;
             };
 
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float2 uv : TEXCOORD0;
                 float3 worldNormal : TEXCOORD1;
                 float3 worldPos : TEXCOORD2;
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             fixed4 _Color;
             float _TextureOffset;
             
             v2f vert (a2v v)
             {
                 v2f o;
 
                 v.vertex.xz += v.normal.xz * _TextureOffset;
		         v.vertex.y = 0;
                 o.pos = UnityObjectToClipPos(v.vertex);

                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                 o.worldNormal = UnityObjectToWorldNormal(v.normal);
                 o.worldPos = mul(unity_ObjectToWorld, v.vertex);
 
                 return o;
             }
             
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed3 worldNormal = normalize(i.worldNormal);
                 fixed3 worldPos = normalize(i.worldPos);
                 fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                 fixed4 texColor = tex2D(_MainTex, i.uv);
                 fixed3 albedo =  texColor.rgb * _Color.rgb;
                 fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * albedo;
                 fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
                 return fixed4(ambient + diffuse, texColor.a);
             }
             ENDCG
         }
     }
 }
