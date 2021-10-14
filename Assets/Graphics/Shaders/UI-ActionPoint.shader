Shader "Custom/UI-ActionPoint"
{   
	Properties
	{
		// Common
		[Enum(Right, 0, Up, 1)] Direction ("Direction", Int) = 0
		BezelWidth("Bezel width", Range(0, 0.5)) = 0.1
		[MaterialToggle] IgnoreBezel("Ignore bezel", Int) = 1

		// Color
		[PerRendererData] Shape("Shape", 2D) = "white" {}
		[PerRendererData] ColorA("Color A", Color) = (0, 1, 1, 1)
		[PerRendererData] ColorB("Color B", Color) = (1, 1, 1, 1)
		[PerRendererData] ColorC("Color C", Color) = (0, 0, 0, 1)
		[PerRendererData] ThresholdA("Threshold A", Range(0, 1)) = 0.25
		[PerRendererData] ThresholdB("Threshold B", Range(0, 1)) = 0.5
		[PerRendererData] Opacity("Opacity", Range(0, 1)) = 1
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
			#pragma vertex VertexFunc
			#pragma fragment FragmentFunc
			#include "UnityCG.cginc"

			// Precision
			#define var		fixed
			#define var2	fixed2
			#define var3	fixed3
			#define var4	fixed4
			#define var3x3	fixed3x3
			// Functions
			void PremultiplyAlpha(inout var4 color)
			{ color.rgb *= color.a; }
			bool Is01(var t)
			{ return t >= 0 && t <= 1; }
			bool Is01(var2 t)
			{ return t.x >= 0 && t.x <= 1 && t.y >= 0 && t.y <= 1; }

			// Common
			sampler2D Shape;
			var4 Shape_ST;
			int Direction;
			var BezelWidth;
			int IgnoreBezel;
			// Color
			var4 ColorA;
			var4 ColorB;
			var4 ColorC;
			var ThresholdA;
			var ThresholdB;
			var Opacity;

			struct v2f
			{
				var4 vertex : SV_POSITION;
				var2 texcoord : TEXCOORD0;
			};

			// Vertex
			v2f VertexFunc(appdata_base input)
			{
				v2f output;
				output.vertex = UnityObjectToClipPos(input.vertex);
				output.texcoord = input.texcoord;
				return output;
			}

			// Fragment
			var4 FragmentFunc(v2f input) : SV_Target
			{
				// Bezel
				var bezelDiv = 1 - BezelWidth;
				var bezelAdd = (1 - 1 / bezelDiv) / 2;
				var2 bezelledCoords = input.texcoord / bezelDiv + bezelAdd;

				// Offsets
				var2 offsetA = var2(1 - ThresholdA, 0);
				var2 offsetB = var2(1 - ThresholdB, 0);
				if(Direction == 1)
				{  
					offsetA = offsetA.yx;
					offsetB = offsetB.yx;
				}
				if(!IgnoreBezel)
				{  
					offsetA /= bezelDiv;
					offsetB /= bezelDiv;
				}
				
				// Masks
				var maskA = Is01(bezelledCoords + offsetA) ? tex2D(Shape, bezelledCoords + offsetA) : 0;
				var maskB = Is01(bezelledCoords + offsetB) ? tex2D(Shape, bezelledCoords + offsetB) : 0;
				var maskBezel = Is01(bezelledCoords) ? tex2D(Shape, bezelledCoords) : 0;
				var maskShape = tex2D(Shape, input.texcoord);

				// Colors
				var4 color = 0;
				color = lerp(color, ColorC, maskShape);
				color = lerp(color, ColorB, maskB * maskBezel);
				color = lerp(color, ColorA, maskA * maskBezel);

				// Return
				PremultiplyAlpha(color);
				return color * Opacity;
			}
			ENDCG
		}
	}
}