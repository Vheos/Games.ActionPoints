// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/UI-TargetingLine" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TextureFlowSpeed ("Texture flow speed", float) = 0
        [Enum(Per Tile, 0, Per Unit, 1)] _TextureFlowMode ("Texture flow mode", int) = 0
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            // Functions
            void PremultiplyAlpha(inout fixed4 color)
			{ color.rgb *= color.a; }

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TextureFlowSpeed;
            int _TextureFlowMode;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed flowOffset = _Time.y * _TextureFlowSpeed;
                if(_TextureFlowMode == 1)
                    flowOffset *= _MainTex_ST.x;

                fixed4 color = tex2D(_MainTex, i.texcoord - flowOffset);
                color *= i.color;
                PremultiplyAlpha(color);
                return  color;
            }
            ENDCG
        }
    }
}