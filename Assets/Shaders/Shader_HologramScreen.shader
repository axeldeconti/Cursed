//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shader_HologramScreen"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
DoodleUV_Size_1("DoodleUV_Size_1", Range(2, 16)) = 4
DoodleUV_Frame_1("DoodleUV_Frame_1", Range(1, 16)) = 2
_Hologram_Value_1("_Hologram_Value_1", Range(0, 1)) = 0.678
_Hologram_Speed_1("_Hologram_Speed_1", Range(0, 4)) = 0.844
__CompressionFX_Value_1("__CompressionFX_Value_1", Range(1, 16)) = 5.295
_MaskAlpha_Fade_1("_MaskAlpha_Fade_1", Range(0, 1)) = 0.691
_CircleFade_PosX_1("_CircleFade_PosX_1", Range(-1, 2)) = 0.511
_CircleFade_PosY_1("_CircleFade_PosY_1", Range(-1, 2)) = 0.365
_CircleFade_Size_1("_CircleFade_Size_1", Range(-1, 1)) = 0.349
_CircleFade_Dist_1("_CircleFade_Dist_1", Range(0, 1)) = 0.542
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
float DoodleUV_Size_1;
float DoodleUV_Frame_1;
float _Hologram_Value_1;
float _Hologram_Speed_1;
float __CompressionFX_Value_1;
float _MaskAlpha_Fade_1;
float _CircleFade_PosX_1;
float _CircleFade_PosY_1;
float _CircleFade_Size_1;
float _CircleFade_Dist_1;

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
inline float Holo1mod(float x,float modu)
{
return x - floor(x * (1.0 / modu)) * modu;
}

inline float Holo1noise(sampler2D source,float2 p)
{
float _TimeX = _Time.y;
float sample = tex2D(source,float2(.2,0.2*cos(_TimeX))*_TimeX*8. + p*1.).x;
sample *= sample;
return sample;
}

inline float Holo1onOff(float a, float b, float c)
{
float _TimeX = _Time.y;
return step(c, sin(_TimeX + a*cos(_TimeX*b)));
}

float4 Hologram(float2 uv, sampler2D source, float value, float speed)
{
float alpha = tex2D(source, uv).a;
float _TimeX = _Time.y * speed;
float2 look = uv;
float window = 1. / (1. + 20.*(look.y - Holo1mod(_TimeX / 4., 1.))*(look.y - Holo1mod(_TimeX / 4., 1.)));
look.x = look.x + sin(look.y*30. + _TimeX) / (50.*value)*Holo1onOff(4., 4., .3)*(1. + cos(_TimeX*80.))*window;
float vShift = .4*Holo1onOff(2., 3., .9)*(sin(_TimeX)*sin(_TimeX*20.) + (0.5 + 0.1*sin(_TimeX*20.)*cos(_TimeX)));
look.y = Holo1mod(look.y + vShift, 1.);
float4 video = float4(0, 0, 0, 0);
float4 videox = tex2D(source, look);
video.r = tex2D(source, look - float2(.05, 0.)*Holo1onOff(2., 1.5, .9)).r;
video.g = videox.g;
video.b = tex2D(source, look + float2(.05, 0.)*Holo1onOff(2., 1.5, .9)).b;
video.a = videox.a;
video = video;
float vigAmt = 3. + .3*sin(_TimeX + 5.*cos(_TimeX*5.));
float vignette = (1. - vigAmt*(uv.y - .5)*(uv.y - .5))*(1. - vigAmt*(uv.x - .5)*(uv.x - .5));
float noi = Holo1noise(source,uv*float2(0.5, 1.) + float2(6., 3.))*value * 3;
float y = Holo1mod(uv.y*4. + _TimeX / 2. + sin(_TimeX + sin(_TimeX*0.63)), 1.);
float start = .5;
float end = .6;
float inside = step(start, y) - step(end, y);
float fact = (y - start) / (end - start)*inside;
float f1 = (1. - fact) * inside;
video += f1*noi;
video += Holo1noise(source,uv*2.) / 2.;
video.r *= vignette;
video *= (12. + Holo1mod(uv.y*30. + _TimeX, 1.)) / 13.;
video.a = video.a + (frac(sin(dot(uv.xy*_TimeX, float2(12.9898, 78.233))) * 43758.5453))*.5;
video.a = (video.a*.3)*alpha*vignette * 2;
video.a *=1.2;
video.a *= 1.2;
video = lerp(tex2D(source, uv), video, value);
return video;
}
float4 Circle_Fade(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
txt.a *= dist;
return txt;
}
float2 DoodleUV(float2 p, float Size, float Speed)
{
float2 co = p;
Speed = (floor(_Time * 20 * Speed) / Speed) * Speed;
co.x = sin((co.x * Size + Speed) * 4);
co.y = cos((co.y * Size + Speed) * 4);
p = lerp(p, p + co, 0.0005 * Size);
return p;
}
float4 frag (v2f i) : COLOR
{
float2 DoodleUV_1 = DoodleUV(i.texcoord,DoodleUV_Size_1,DoodleUV_Frame_1);
float4 _Hologram_1 = Hologram(DoodleUV_1,_MainTex,_Hologram_Value_1,_Hologram_Speed_1);
float4 _CompressionFX_1 = CompressionFX(i.texcoord,_MainTex,__CompressionFX_Value_1);
float4 MaskAlpha_1=_Hologram_1;
MaskAlpha_1.a = lerp(_CompressionFX_1.a * _Hologram_1.a, (1 - _CompressionFX_1.a) * _Hologram_1.a,_MaskAlpha_Fade_1);
float4 _CircleFade_1 = Circle_Fade(MaskAlpha_1,i.texcoord,_CircleFade_PosX_1,_CircleFade_PosY_1,_CircleFade_Size_1,_CircleFade_Dist_1);
float4 FinalResult = _CircleFade_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}