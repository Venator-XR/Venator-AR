Shader "Custom/DepthOccluder"
{
    SubShader
    {
        Tags { "Queue"="Geometry-1" "RenderType"="Opaque" }
        Lighting Off
        Cull Back
        ZWrite On
        ColorMask 0

        Pass { }
    }
}