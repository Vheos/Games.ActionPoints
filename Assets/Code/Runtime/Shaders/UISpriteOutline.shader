Shader "Custom/UISpriteOutline" 
{
    Properties
    {
       _MainTex ("Texture", 2D) = "white"
       [PerRendererData] Thickness ("Thickness", Float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex VertexFunction
            #pragma fragment FragmentFunction
            #include "Utility.cginc"

            // Properties
            sampler2D _MainTex;
            fixed4 _MainTex_TexelSize;
            fixed Thickness;

            // Functions
            struct VertexData
            {
                fixed4 vertex   : POSITION;
                fixed4 color    : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };             
            VertexData VertexFunction(VertexData data)
            {
                data.vertex = UnityObjectToClipPos(data.vertex);         
                return data;
            }
            fixed4 FragmentFunction(VertexData data) : SV_Target
            {
                fixed2 offset = fixed2(Thickness, 0);
                fixed alphaMain =  tex2D(_MainTex, data.texcoord).a;
                fixed alphaLeft = tex2D(_MainTex, data.texcoord + offset.xy * _MainTex_TexelSize).a;
                fixed alphaRight = tex2D(_MainTex, data.texcoord - offset.xy * _MainTex_TexelSize).a;
                fixed alphaDown = tex2D(_MainTex, data.texcoord + offset.yx * _MainTex_TexelSize).a;
                fixed alphaUp = tex2D(_MainTex, data.texcoord - offset.yx * _MainTex_TexelSize).a;
                fixed alpha = min(1 - alphaMain, Max(alphaLeft, alphaRight, alphaDown, alphaUp)); 

                // rotation
                fixed s = sin ( _Time.x);
                fixed c = cos ( _Time.x);           
                fixed2x2 rotation = float2x2(c, -s, s, c);

                return PremultipliedAlpha(data.color) * alpha;
            }
            ENDCG
        }
    }
}