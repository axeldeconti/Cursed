//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shader_CreatureOnCharacter"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
AnimatedTwistUV_AnimatedTwistUV_Bend_1("AnimatedTwistUV_AnimatedTwistUV_Bend_1", Range(-1, 1)) = 0.84
AnimatedTwistUV_AnimatedTwistUV_PosX_1("AnimatedTwistUV_AnimatedTwistUV_PosX_1", Range(-1, 2)) = -0.111
AnimatedTwistUV_AnimatedTwistUV_PosY_1("AnimatedTwistUV_AnimatedTwistUV_PosY_1", Range(-1, 2)) = 0.255
AnimatedTwistUV_AnimatedTwistUV_Radius_1("AnimatedTwistUV_AnimatedTwistUV_Radius_1", Range(0, 1)) = 0.5
AnimatedTwistUV_AnimatedTwistUV_Speed_1("AnimatedTwistUV_AnimatedTwistUV_Speed_1", Range(-10, 10)) = 0.254
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 3.258
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 12.567
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 1
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 1
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = 0.592
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
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
float AnimatedTwistUV_AnimatedTwistUV_Bend_1;
float AnimatedTwistUV_AnimatedTwistUV_PosX_1;
float AnimatedTwistUV_AnimatedTwistUV_PosY_1;
float AnimatedTwistUV_AnimatedTwistUV_Radius_1;
float AnimatedTwistUV_AnimatedTwistUV_Speed_1;
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
sampler2D _NewTex_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedPingPongOffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
{
float time = sin(_Time * 100* speed)  * 0.1;
speed *= time * 25;
uv += float2(offsetx, offsety)*speed;
uv = uv * float2(zoomx, zoomy);
return uv;
}
float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float2 AnimatedTwistUV(float2 uv, float value, float posx, float posy, float radius, float speed)
{
float2 center = float2(posx, posy);
float2 tc = uv - center;
float dist = length(tc);
if (dist < radius)
{
float percent = (radius - dist) / radius;
float theta = percent * percent * 16.0 * sin(value);
float s = sin(theta + (_Time.y * speed));
float c = cos(theta + (_Time.y * speed));
tc = float2(dot(tc, float2(c, -s)), dot(tc, float2(s, c)));
}
tc += center;
return tc;
}
float4 frag (v2f i) : COLOR
{
float2 AnimatedTwistUV_1 = AnimatedTwistUV(i.texcoord,AnimatedTwistUV_AnimatedTwistUV_Bend_1,AnimatedTwistUV_AnimatedTwistUV_PosX_1,AnimatedTwistUV_AnimatedTwistUV_PosY_1,AnimatedTwistUV_AnimatedTwistUV_Radius_1,AnimatedTwistUV_AnimatedTwistUV_Speed_1);
float2 DistortionUV_1 = DistortionUV(AnimatedTwistUV_1,DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float4 NewTex_1 = tex2D(_NewTex_1,DistortionUV_1);
float4 FinalResult = NewTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
