//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shader_Props_TextMonitor"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
PixelUV_Size_1("PixelUV_Size_1", Range(1, 128)) = 83
AnimatedShakeUV_1_OffsetX_1("AnimatedShakeUV_1_OffsetX_1", Range(0, 0.05)) = 0.02
AnimatedShakeUV_1_OffsetY_1("AnimatedShakeUV_1_OffsetY_1", Range(0, 0.05)) = 0
AnimatedShakeUV_1_IntenseX_1("AnimatedShakeUV_1_IntenseX_1", Range(-3, 3)) = 3
AnimatedShakeUV_1_IntenseY_1("AnimatedShakeUV_1_IntenseY_1", Range(-3, 3)) = 1
AnimatedShakeUV_1_Speed_1("AnimatedShakeUV_1_Speed_1", Range(-1, 1)) = 1
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
__CompressionFX_Value_1("__CompressionFX_Value_1", Range(1, 16)) = 16
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float PixelUV_Size_1;
float AnimatedShakeUV_1_OffsetX_1;
float AnimatedShakeUV_1_OffsetY_1;
float AnimatedShakeUV_1_IntenseX_1;
float AnimatedShakeUV_1_IntenseY_1;
float AnimatedShakeUV_1_Speed_1;
sampler2D _SourceNewTex_1;
float __CompressionFX_Value_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float CMPFXrng2(float2 seed)
{
return frac(sin(dot(seed * floor(50 + (_Time + 0.1) * 12.), float2(127.1, 311.7))) * 43758.5453123);
}

float CMPFXrng(float seed)
{
return CMPFXrng2(float2(seed, 1.0));
}

float4 CompressionFX(float2 uv, sampler2D source,float Value)
{
float2 blockS = floor(uv * float2(24., 19.))*4.0;
float2 blockL = floor(uv * float2(38., 14.))*4.0;
float r = CMPFXrng2(uv);
float lineNoise = pow(CMPFXrng2(blockS), 3.0) *Value* pow(CMPFXrng2(blockL), 3.0);
float4 col1 = tex2D(source, uv + float2(lineNoise * 0.02 * CMPFXrng(2.0), 0));
float4 result = float4(float3(col1.x, col1.y, col1.z), 1.0);
result.a = col1.a;
return result;
}
float2 PixelUV(float2 uv, float x)
{
uv = floor(uv * x + 0.5) / x;
return uv;
}
float2 AnimatedShakeUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
{
float time = sin(_Time * speed * 5000 * zoomx);
float time2 = sin(_Time * speed * 5000 * zoomy);
uv += float2(offsetx * time, offsety * time2);
return uv;
}
float4 frag (v2f i) : COLOR
{
float2 PixelUV_1 = PixelUV(i.texcoord,PixelUV_Size_1);
float2 AnimatedShakeUV_1 = AnimatedShakeUV(PixelUV_1,AnimatedShakeUV_1_OffsetX_1,AnimatedShakeUV_1_OffsetY_1,AnimatedShakeUV_1_IntenseX_1,AnimatedShakeUV_1_IntenseY_1,AnimatedShakeUV_1_Speed_1);
float4 _CompressionFX_1 = CompressionFX(AnimatedShakeUV_1,_SourceNewTex_1,__CompressionFX_Value_1);
float4 FinalResult = _CompressionFX_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
