//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shader_Props_MonitorGlitch"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
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
inline float RBFXmod(float x,float modu)
{
return x - floor(x * (1.0 / modu)) * modu;
}

float3 RBFXrainbow(float t)
{
t= RBFXmod(t,1.0);
float tx = t * 8;
float r = clamp(tx - 4.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float g = tx < 2.0 ? clamp(tx, 0.0, 1.0) : clamp(4.0 - tx, 0.0, 1.0);
float b = tx < 4.0 ? clamp(tx - 2.0, 0.0, 1.0) : clamp(6.0 - tx, 0.0, 1.0);
return float3(r, g, b);
}

float4 Plasma(float4 txt, float2 uv, float _Fade, float speed)
{
float _TimeX=_Time.y * speed;
float a = 1.1 + _TimeX * 2.25;
float b = 0.5 + _TimeX * 1.77;
float c = 8.4 + _TimeX * 1.58;
float d = 610 + _TimeX * 2.03;
float x1 = 2.0 * uv.x;
float n = sin(a + x1) + sin(b - x1) + sin(c + 2.0 * uv.y) + sin(d + 5.0 * uv.y);
n = RBFXmod(((5.0 + n) / 5.0), 1.0);
float4 nx=txt;
n += nx.r * 0.2 + nx.g * 0.4 + nx.b * 0.2;
float4 ret=float4(RBFXrainbow(n),txt.a);
return lerp(txt,ret,_Fade);
}
float4 frag (v2f i) : COLOR
{
float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
float4 FinalResult = _MainTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
