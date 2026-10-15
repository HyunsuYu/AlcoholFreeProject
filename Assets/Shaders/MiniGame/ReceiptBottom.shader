Shader "AF/ReceiptBottom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Wave Settings)]
        _WaveSpeed ("Wave Speed", float) = 1.0

        _WaveAmplitude ("Wave Amplitude", float) = 0.0
        _WaveYPos ("Wave Y Position", float) = 0.0
        _WaveFrequency ("Wave Frequency", float) = 1.0
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

            float _WaveSpeed;

            float _WaveAmplitude;
            float _WaveYPos;
            float _WaveFrequency;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 result = fixed4(1.0f, 1.0f, 1.0f, 1.0f);

                float forwardVal = (_WaveAmplitude / 0.5f) * (0.5f - abs(((_WaveSpeed * _Time.y) + (_WaveFrequency * i.uv.x * 10.0f)) % (2 * 0.5f) - 0.5f)) + _WaveYPos;
                if (forwardVal > i.uv.y)
                {
                    result = fixed4(1.0f, 1.0f, 1.0f, 0.0f);
                }

                return result;
            }
            ENDCG
        }
    }
}
