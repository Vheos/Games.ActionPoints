Shader "Custom/UISpriteOutline" 
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
        _Thickness ("Thickness", Float) = 0.1
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
            fixed _Thickness;

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
                fixed2 offset = fixed2(_Thickness, 0);
                fixed2 ratio = _MainTex_TexelSize.zw / max(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                fixed alphaMain =  tex2D(_MainTex, data.texcoord).a;
                fixed alphaLeft = tex2D(_MainTex, data.texcoord + offset.xy / ratio).a;
                fixed alphaRight = tex2D(_MainTex, data.texcoord - offset.xy / ratio).a;
                fixed alphaDown = tex2D(_MainTex, data.texcoord + offset.yx / ratio).a;
                fixed alphaUp = tex2D(_MainTex, data.texcoord - offset.yx / ratio).a;
                fixed alpha = min(1 - alphaMain, max(alphaLeft, max(alphaRight, max(alphaDown, alphaUp)))); 
                return PremultipliedAlpha(data.color) * alpha;
            }
            ENDCG
        }
    }
}