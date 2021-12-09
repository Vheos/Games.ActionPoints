Shader "Custom/UIActionPoint"
{   
	Properties
	{
		// Per par
		[Enum(Right, 0, Up, 1)] Direction ("Direction", Int) = 0
		BezelWidth("Bezel width", Range(0, 0.5)) = 0.1
		[MaterialToggle] IgnoreBezel("Ignore bezel", Int) = 1
		[PerRendererData] ColorA("Color A", Color) = (1, 1, 1, 1)
		[PerRendererData] ColorB("Color B", Color) = (1, 1, 1, 1)
		[PerRendererData] ColorC("Color C", Color) = (1, 1, 1, 1)
		// Per point
		[PerRendererData] Shape("Shape", 2D) = "white" {}	
		[PerRendererData] Opacity("Opacity", Range(0, 1)) = 0
		[PerRendererData] ThresholdA("Threshold A", Range(0, 1)) = 0
		[PerRendererData] ThresholdB("Threshold B", Range(0, 1)) = 0	
	}

	SubShader
	{
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
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
			#include "UnityCG.cginc"
			#include "Utility.cginc"

			// Properties
			int Direction;
			fixed BezelWidth;
			int IgnoreBezel;
			fixed4 ColorA;
			fixed4 ColorB;
			fixed4 ColorC;
			sampler2D Shape;
			fixed4 Shape_ST;
			fixed ThresholdA;
			fixed ThresholdB;
			fixed Opacity;

            // Structs
            struct VertexData
            {
                fixed4 vertex   : POSITION;
                fixed2 texcoord : TEXCOORD0;
            };  

			// Functions
			VertexData VertexFunction(VertexData input)
			{
				input.vertex = UnityObjectToClipPos(input.vertex);
				return input;
			}
			fixed4 FragmentFunction(VertexData input) : SV_Target
			{
				// Bezel
				fixed bezelDiv = 1 - BezelWidth;
				fixed bezelAdd = (1 - 1 / bezelDiv) / 2;
				fixed2 bezelledCoords = input.texcoord / bezelDiv + bezelAdd;

				// Offsets
				fixed2 offsetA = fixed2(1 - ThresholdA, 0);
				fixed2 offsetB = fixed2(1 - ThresholdB, 0);
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
				fixed maskA = Is01(bezelledCoords + offsetA) ? tex2D(Shape, bezelledCoords + offsetA) : 0;
				fixed maskB = Is01(bezelledCoords + offsetB) ? tex2D(Shape, bezelledCoords + offsetB) : 0;
				fixed maskBezel = Is01(bezelledCoords) ? tex2D(Shape, bezelledCoords) : 0;
				fixed maskShape = tex2D(Shape, input.texcoord);

				// Colors
				fixed4 color = 0;
				color = lerp(color, ColorC, maskShape);
				color = lerp(color, ColorB, maskB * maskBezel);
				color = lerp(color, ColorA, maskA * maskBezel);

				// Return
				return PremultipliedAlpha(color) * Opacity;
			}
			ENDCG
		}
	}
}