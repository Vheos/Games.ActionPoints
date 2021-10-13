// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/UI-SpriteRenderer" 
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _Thickness ("Thickness", Float) = 0.1
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            // Functions
            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            { return float4(pos.xy * flip, pos.z, 1.0); }

            // Properties
            fixed4 _Color;
            sampler2D _MainTex;
            fixed4 _MainTex_TexelSize;
            sampler2D _AlphaTex;

            #ifdef UNITY_INSTANCING_ENABLED
                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    // this could be smaller but that's how bit each entry is regardless of type
                    UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                    UNITY_DEFINE_INSTANCED_PROP(fixed, Instanced_Thickness)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
                #define _Thickness      UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, Instanced_Thickness)
            #endif

            CBUFFER_START(UnityPerDrawSprite)
                #ifndef UNITY_INSTANCING_ENABLED
                    fixed4 _RendererColor;
                    fixed2 _Flip;
                    fixed _Thickness;
                #endif
                    float _EnableExternalAlpha;
            CBUFFER_END



            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 custom   : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 custom   : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                OUT.custom.x = _Thickness;
                OUT.custom.y = 0;
                return OUT;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = IN.color;
                #if ETC1_EXTERNAL_ALPHA
                    fixed4 alpha = tex2D(_AlphaTex, uv);
                    c.a = lerp(c.a, alpha.r, _EnableExternalAlpha);
                #endif       

                fixed2 offset = fixed2(IN.custom.x, 0);
                fixed2 ratio = _MainTex_TexelSize.zw / max(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                fixed alphaMain =  tex2D(_MainTex, IN.texcoord).a;
                fixed alphaLeft = tex2D(_MainTex, IN.texcoord + offset.xy / ratio).a;
                fixed alphaRight = tex2D(_MainTex, IN.texcoord - offset.xy / ratio).a;
                fixed alphaDown = tex2D(_MainTex, IN.texcoord + offset.yx / ratio).a;
                fixed alphaUp = tex2D(_MainTex, IN.texcoord - offset.yx / ratio).a;
                fixed alphaMax = max(alphaMain, max(alphaLeft, max(alphaRight, max(alphaDown, alphaUp))));                

                c.rgb *= c.a;
                return c * alphaMax;
            }

            ENDCG
        }
    }
}