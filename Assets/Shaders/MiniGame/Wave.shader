Shader "Unlit/Wave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}

        _WaveSpeed ("Wave Speed", float) = 1.0

        _WaveAmplitude ("Wave Amplitude", float) = 0.0
        // _WaveYPos ("Wave Y Position", float) = 0.0
        _WaveFrequency ("Wave Frequency", float) = 1.0

        _LiqureTopYPos ("Liquid Top Y Position", float) = 0.0
        _LiqureBottomYPos ("Liquid Bottom Y Position", float) = 0.0

        // _FrontTopColor ("Front Top Color", Color) = (1, 0, 0, 1)
        // _FrontBottomColor ("Front Bottom Color", Color) = (1, 0.5, 0, 1)

        // _BackTopColor ("Back Top Color", Color) = (0, 1, 0, 1)
        // _BackBottomColor ("Back Bottom Color", Color) = (0, 0.5, 1, 1)

        _LiqureOpacity ("Liquid Opacity", Range(0.0, 1.0)) = 0.15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            sampler2D _MaskTex;
            float4 _MainTex_ST;

            float _WaveSpeed;

            float _WaveAmplitude;
            // float _WaveYPos;
            float _WaveFrequency;

            float _LiqureTopYPos;
            float _LiqureBottomYPos;

            float _LiqureOpacity;

            // fixed4 _FrontTopColor;
            // fixed4 _FrontBottomColor;

            // fixed4 _BackTopColor;
            // fixed4 _BackBottomColor;

            float _WaveYPoses[10];
            fixed4 _FrontTopColors[10];
            fixed4 _FrontBottomColors[10];
            fixed4 _BackTopColors[10];
            fixed4 _BackBottomColors[10];
            half _WaveLayerCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                float mask = tex2D(_MaskTex, i.uv).r;

                fixed4 result = mainTex;
                if(mask == 1.0 && _LiqureTopYPos + _WaveAmplitude >= i.uv.y && i.uv.y >= _LiqureBottomYPos - _WaveAmplitude)
                {
                    for(int index = 0; index < _WaveLayerCount; index++)
                    {
                        float curWavePos = (_LiqureTopYPos - _LiqureBottomYPos) * _WaveYPoses[index] + _LiqureBottomYPos;
                        float gradientRatio = pow((i.uv.y - _LiqureBottomYPos) / (_LiqureTopYPos - _LiqureBottomYPos), 2.0) + pow(1 - _WaveYPoses[0], 2.0);

                        int colorIndex = _WaveLayerCount - index - 1;
                        if (index == 0 && _WaveAmplitude * sin(_WaveSpeed * _Time.y + _WaveFrequency * i.uv.x) + curWavePos > i.uv.y)
                        {
                            result = lerp(_BackBottomColors[colorIndex], _BackTopColors[colorIndex], gradientRatio);
                            result = lerp(result, mainTex, _LiqureOpacity);
                        }
                        if (_WaveAmplitude * sin(-1.0 * _WaveSpeed * _Time.y + _WaveFrequency * i.uv.x + (((index + 1) * 3.14159265) / 2.0)) + curWavePos > i.uv.y)
                        {
                            result = lerp(_FrontBottomColors[colorIndex], _FrontTopColors[colorIndex], gradientRatio);
                            result = lerp(result, mainTex, _LiqureOpacity);
                        }
                    }

                    // float curWavePos = (_LiqureTopYPos - _LiqureBottomYPos) * _WaveYPos + _LiqureBottomYPos;
                    // float gradientRatio = pow((i.uv.y - _LiqureBottomYPos) / (_LiqureTopYPos - _LiqureBottomYPos), 2.0) + pow(1 - _WaveYPos, 2.0);
                    // if (_WaveAmplitude * sin(_WaveSpeed * _Time.y + _WaveFrequency * i.uv.x) + curWavePos > i.uv.y)
                    // {
                    //     result = lerp(_BackBottomColor, _BackTopColor, gradientRatio);
                    //     result = lerp(result, mainTex, 0.35);
                    // }
                    // if (_WaveAmplitude * sin(-1.0 * _WaveSpeed * _Time.y + _WaveFrequency * i.uv.x + 3.14159265 / 2.0) + curWavePos > i.uv.y)
                    // {
                    //     result = lerp(_FrontBottomColor, _FrontTopColor, gradientRatio);
                    //     result = lerp(result, mainTex, 0.35);
                    // }
                }

                return result;
            }
            ENDCG
        }
    }
}
