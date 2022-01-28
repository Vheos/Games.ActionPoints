Shader "Custom/ActionPoint"
{   
	Properties
	{
		// Per bar
		BezelWidth("Bezel width", Range(0, 0.5)) = 0
		FocusColor("Focus color", Color) = (0, 1, 1, 1)
		ActionColor("Action color", Color) = (1, 1, 1, 1)
		ExhaustColor("Exhaust color", Color) = (1, 0, 0, 1)
		BackgroundColor("Background color", Color) = (0, 0, 0, 1)

		// Per point
		[PerRendererData] Shape("Shape", 2D) = "white" {}	
		[PerRendererData] Opacity("Opacity", Range(0, 1)) = 0
		[PerRendererData] FocusProgress("Focus progress", Range(0, 1)) = 0
		[PerRendererData] ActionProgress("Action progress", Range(-1, 1)) = 0
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
			fixed4 FocusColor;
			fixed4 ActionColor;
			fixed4 ExhaustColor;
			fixed4 BackgroundColor;
			sampler2D Shape;
			fixed FocusProgress;
			fixed ActionProgress;
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
				fixed4 chosenActionColor = ActionProgress >= 0 ? ActionColor : ExhaustColor;
				fixed absActionProgress = abs(ActionProgress);
				fixed2 offsetA = fixed2(1 - FocusProgress, 0);
				fixed2 offsetB = fixed2(1 - absActionProgress, 0);
				
				// Masks
				fixed maskA = Is01(bezelledCoords + offsetA) ? tex2D(Shape, bezelledCoords + offsetA) : 0;
				fixed maskB = Is01(bezelledCoords + offsetB) ? tex2D(Shape, bezelledCoords + offsetB) : 0;
				fixed maskBezel = Is01(bezelledCoords) ? tex2D(Shape, bezelledCoords) : 0;
				fixed maskShape = tex2D(Shape, input.texcoord);

				// Colors
				fixed4 color = 0;
				color = lerp(color, BackgroundColor, maskShape);
				color = lerp(color, chosenActionColor, maskB * maskBezel);
				color = lerp(color, FocusColor, maskA * maskBezel);

				// Return
				return PremultipliedAlpha(color) * Opacity;
			}
			ENDCG
		}
	}
}