//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shader_LaunchTarget"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 91.228
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 93.556
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.15
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 1
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 2
_Burn_Value_1("_Burn_Value_1", Range(0, 1)) = 0.386
_Burn_Speed_1("_Burn_Speed_1", Range(-8, 8)) = 0.907
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
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
float _Burn_Value_1;
float _Burn_Speed_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float BFXr (float2 c, float seed)
{
return frac(43.*sin(c.x+7.*c.y)* seed);
}

float BFXn (float2 p, float seed)
{
float2 i = floor(p), w = p-i, j = float2 (1.,0.);
w = w*w*(3.-w-w);
return lerp(lerp(BFXr(i, seed), BFXr(i+j, seed), w.x), lerp(BFXr(i+j.yx, seed), BFXr(i+1., seed), w.x), w.y);
}

float BFXa (float2 p, float seed)
{
float m = 0., f = 2.;
for ( int i=0; i<9; i++ ){ m += BFXn(f*p, seed)/f; f+=f; }
return m;
}

float4 BurnFX(float4 txt, float2 uv, float value, float seed, float HDR)
{
float t = frac(value*0.9999);
float4 c = smoothstep(t / 1.2, t + .1, BFXa(3.5*uv, seed));
c = txt*c;
c.r = lerp(c.r, c.r*15.0*(1 - c.a), value);
c.g = lerp(c.g, c.g*10.0*(1 - c.a), value);
c.b = lerp(c.b, c.b*5.0*(1 - c.a), value);
c.rgb += txt.rgb*value;
c.rgb = lerp(saturate(c.rgb),c.rgb,HDR);
return c;
}
float4 frag (v2f i) : COLOR
{
float2 DistortionUV_1 = DistortionUV(i.texcoord,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float4 _MainTex_1 = tex2D(_MainTex,DistortionUV_1);
float4 _Burn_1 = BurnFX(_MainTex_1,i.texcoord,_Burn_Value_1,_Burn_Speed_1,0);
float4 FinalResult = _Burn_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
