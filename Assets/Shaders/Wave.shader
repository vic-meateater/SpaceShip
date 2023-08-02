Shader "Unlit/Wave"
{
    Properties
    {
    _YOffset("YOffset", Range(-10, 10)) = -1 //смещение по Y
    _Height("Height", Range(0,20)) = 0.5 // сила изгиба
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            float _Height; // сила изгиба
            float _YOffset;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            //вершинный
            v2f vert (appdata_full v)
            {
                v2f o;
                v.vertex.xyz += sin(v.normal * _Height * v.texcoord.x);
                v.vertex.y += _YOffset;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               
                return o;
            }
            //фрагментарный
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
