Shader "SP/InvisibleShadowReceiver_URP"
{
    Properties
    {
        _ShadowColor("Shadow Tint", Color) = (0, 0, 0, 0.5)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        Pass
        {
            Name "ForwardLit"
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
                // Sombra principal (0 = en sombra, 1 = iluminado)
                float shadowAtten = MainLightRealtimeShadow(i.shadowCoord);
                // Queremos que se vea solo la sombra (invisible al resto)
                float shadowDarkness = 1.0 - shadowAtten;
                half3 shadow = _ShadowColor.rgb * shadowDarkness;
                return half4(shadow, shadowDarkness * _ShadowColor.a);
            }
            ENDHLSL
        }
    }
}
