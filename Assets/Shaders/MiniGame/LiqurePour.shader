Shader "LiqurePour"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Radius ("Radius", Range(0, 100000)) = 1.0

        _MinCut ("MinCut", Range(0, 1)) = 0000001

        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _MetaballPoses[20];
            int _MetaballPosCount;

            float _MinCut;

            fixed4 _Color;

            float getMetaball(fixed2 a, fixed2 b);

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv);

                float sum = 0.0f;
                for(int index = 0; index < _MetaballPosCount; index++)
                {
                   sum += saturate(getMetaball(i.uv, _MetaballPoses[index].xy) / _Radius);
                }
                sum = saturate(sum);

                if(sum >= _MinCut)
                {
                    return fixed4(sum * _Color.r, sum * _Color.g, sum * _Color.b, 1.0f);
                }

                return fixed4(0.0f, 0.0f, 0.0f, 0.0f);
            }

            float getMetaball(fixed2 a, fixed2 b)
            {
                return 1.0f / (pow(b.x - a.x, 2.0f) + pow(b.y - a.y, 2.0f));
            }
            ENDCG
        }
    }
}
