Shader "ActionPoint"
{   
	Properties
	{
		// Common
		Shape("Shape", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)] FaceCulling("Face Culling", Int) = 2
		// Color
		[PerRendererData] BackgroundTint("Background Tint", Color) = (0.5, 0.5, 0.5, 1)
		[PerRendererData] ProgressTint("Progress Tint", Color) = (1, 1, 1, 1)
		[PerRendererData] Opacity("Opacity", Range(0, 1)) = 1
		// Control
		[PerRendererData] Progress("Progress", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		ZWrite Off
		Blend One OneMinusSrcAlpha
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
			// Constants
			#define WorldScaleX		unity_ObjectToWorld._m00_m10_m20
			#define WorldScaleY		unity_ObjectToWorld._m01_m11_m21

			// Common
			sampler2D Shape;
			var4 Shape_ST;
			// Color
			var4 ProgressTint;
			var4 BackgroundTint;
			var Opacity;
			// Control
			var Progress;
			
			// Functions
			var2 Rotate(var2 v, var radians)
			{
				var s = -sin(radians);
				var c = cos(radians);
				return var2(v.x * c - v.y * s, v.x * s + v.y * c);
			}
			void PremultiplyAlpha(inout var4 color)
			{ color.rgb *= color.a; }
			var4 BlendPremultiplied(var4 front, var4 back)
			{ return front + back * (1 - front.a); }
			var4 BlendPremultiplied(var4 a, var4 b, var4 c)
			{ return BlendPremultiplied(a, BlendPremultiplied(b, c)); }
			var Map(var t, var a, var b, var c, var d)
			{ return (t - a) * (d - c) / (b - a) + c; }
			var MapTo01(var t, var a, var b)
			{ return Map(t, a, b, 0, 1); }
			var MapFrom01(var t, var a, var b)
			{ return Map(t, 0, 1, a, b); }
			var2 Map(var2 t, var2 a, var2 b, var2 c, var2 d)
			{ return (t - a) * (d - c) / (b - a) + c; }
			var2 MapTo01(var2 t, var2 a, var2 b)
			{ return Map(t, a, b, 0, 1); }
			var2 MapFrom01(var2 t, var2 a, var2 b)
			{ return Map(t, 0, 1, a, b); }

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
				// Cache	
				var4 mask = tex2D(Shape, input.texcoord);
				var4 backgroundColor = mask * BackgroundTint;

				// Progress
				var4 progressColor = 0;
				if(input.texcoord.x <= Progress)
					progressColor =  mask * ProgressTint;
				
				// Premultiply
				PremultiplyAlpha(backgroundColor);
				PremultiplyAlpha(progressColor);
				return BlendPremultiplied(progressColor, backgroundColor) * Opacity;
			}
			ENDCG
		}
	}
}