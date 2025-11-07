Shader "SP/InvisibleShadowReceiver_URP"
{
    Properties
    {
        _ShadowColor("Shadow Tint", Color) = (0, 0, 0, 0.5)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" } // Dibujado antes que los transparentes
        ZWrite On
        ZTest LEqual
        ColorMask 0 // No dibuja color, pero sí escribe profundidad

        Pass
        {
            Name "ShadowOnly"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float4 shadowCoord : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(o.positionWS);
                o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
                return o;
            }

            float4 _ShadowColor;

            half4 frag(Varyings i) : SV_Target
            {
                float shadowAtten = MainLightRealtimeShadow(i.shadowCoord);
                float shadowDarkness = 1.0 - shadowAtten;

                // Solo sombras (invisible pero con depth)
                return half4(_ShadowColor.rgb * shadowDarkness, shadowDarkness * _ShadowColor.a);
            }
            ENDHLSL
        }
    }
}
