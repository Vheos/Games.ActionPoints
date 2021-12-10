Shader "Custom/UITargetingLine" 
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
        TextureFlowSpeed ("Texture flow speed", float) = 0
        [Enum(Per Tile, 0, Per Unit, 1)] TextureFlowMode ("Texture flow mode", int) = 0
        [PerRendererData] TilingX ("Tiling X", Float) = 1
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
            fixed TextureFlowSpeed;
            int TextureFlowMode;
            fixed TilingX;

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
                fixed flowOffset = _Time.y * TextureFlowSpeed;
                if(TextureFlowMode == 1)
                    flowOffset *= TilingX;

                fixed2 finalTexcoord = data.texcoord * fixed2(TilingX, 1) - flowOffset;
                fixed4 color = tex2D(_MainTex, finalTexcoord) * data.color;             
                return PremultipliedAlpha(color);
            }
            ENDCG
        }
    }
}