Shader "Custom/URP/VaseFill"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,0,0,1)
        _FillAmount("Fill Amount", Range(0,1)) = 0
        _MinY("MinY", Float) = 0
        _MaxY("MaxY", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 normalOS : TEXCOORD1;
            };

            float4 _BaseColor;
            float _FillAmount, _MinY, _MaxY;

            Varyings vert(Attributes input)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                o.localPos = input.positionOS.xyz;
                o.normalOS = input.normalOS;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float cutoff = lerp(_MinY, _MaxY, _FillAmount);

                // Allow top cap (normals pointing up)
                if (i.normalOS.y > 0.8)
                    return _BaseColor;

                // Bottom-to-top fill
                clip(cutoff - i.localPos.y);

                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
