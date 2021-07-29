Shader "CSprite"
{   
	Properties
	{
		 [PerRendererData] Texture("Texture", 2D) = "white" {}
		 [PerRendererData] WhiteTint("White Tint", Color) = (1, 1, 1, 1)
		 [PerRendererData] BlackTint("Black Tint", Color) = (0, 0, 0, 1)
		 [Enum(UnityEngine.Rendering.CullMode)] FaceCulling("Face Culling", Int) = 2
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull [FaceCulling] 

		Pass
		{
			CGPROGRAM 
			#pragma vertex VertexFunc
			#pragma fragment FragmentFunc
			#include "UnityCG.cginc"

			// Precision
			#define var		fixed
			#define var2	fixed2
			#define var3	fixed3
			#define var4	fixed4
			#define var3x3	fixed3x3
			// Properties
			sampler2D Texture;
			var4 Texture_ST;
			var4 WhiteTint;
			var4 BlackTint;

			// Vertex output
			struct v2f
			{
				var4 vertex : SV_POSITION;
				var2 texcoord : TEXCOORD0;
			};

			// Vertex shader
			v2f VertexFunc(appdata_base input)
			{
				v2f output;
				output.vertex = UnityObjectToClipPos(input.vertex);
				output.texcoord = TRANSFORM_TEX(input.texcoord, Texture);
				return output;
			}

			// Fragment shader
			var4 FragmentFunc(v2f input) : SV_Target
			{
				var4 color = tex2D(Texture, input.texcoord);
				var3 tintedColor = lerp(BlackTint.rgb, WhiteTint.rgb, color.rgb);
				return var4(tintedColor, color.a);
			}
			ENDCG
		}
	}
}