Shader "Custom/UISprite" 
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
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

            // Structs
            struct VertexData
            {
                fixed4 vertex   : POSITION;
                fixed4 color    : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };  

            // Functions           
            VertexData VertexFunction(VertexData data)
            {
                data.vertex = UnityObjectToClipPos(data.vertex);              
                return data;
            }
            fixed4 FragmentFunction(VertexData data) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, data.texcoord) * data.color;             
                return PremultipliedAlpha(color);
            }
            ENDCG
        }
    }
}