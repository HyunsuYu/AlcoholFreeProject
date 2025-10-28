Shader "ScreenTransition/ScreenBrushTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BeforeTex ("Before Texture", 2D) = "white" {}
        _AfterTex ("After Texture", 2D) = "white" {}

        _Radius ("Radius", Range(0, 100000)) = 1.0
        _MinCut ("MinCut", Range(0, 1)) = 0000001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BeforeTex;
            sampler2D _AfterTex;

            float4 _Routes[1000];
            int _RouteCount;

            float _Radius;
            float _MinCut;

            float getMetaball(fixed2 a, fixed2 b);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 beforeTexColor = tex2D(_BeforeTex, i.uv);
                fixed4 afterTexColor = tex2D(_AfterTex, i.uv);

                float sum = 0.0f;
                for(int index = 0; index < _RouteCount; index++)
                {
                   sum += saturate(getMetaball(i.uv, _Routes[index].xy) / _Radius);
                }
                sum = saturate(sum);

                if(sum >= _MinCut)
                {
                    return lerp(beforeTexColor, afterTexColor, sum);
                }

                return beforeTexColor;
            }

            float getMetaball(fixed2 a, fixed2 b)
            {
                return 1.0 / (length(a - b) * length(a - b));
            }
            ENDCG
        }
    }
}
