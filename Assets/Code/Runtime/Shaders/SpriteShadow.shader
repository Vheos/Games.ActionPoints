// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SpriteShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        ShadowAlphaCutoff("Shadow alpha cutoff", Range(0, 1)) = 0.5
        IncidenceMin("Incidence minimum", Range(0, 1)) = 0.5
        IncidenceSharpness("Incidence sharpness", Range(1, 10)) = 2
        ViewDirectionOffset("View direction offset", Range(-10, 10)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface SurfaceFunction LightingFunction vertex:VertexFunction addshadow fullforwardshadows alphatest:ShadowAlphaCutoff keepalpha nofog nolightmap nodynlightmap noinstancing 
        #include "Utility.cginc"

        sampler2D _MainTex;
        fixed IncidenceMin;
        fixed IncidenceSharpness;
        fixed ViewDirectionOffset;

        // Structs
        struct VertexData
        {
            fixed4 vertex   : POSITION;
            fixed3 normal   : NORMAL;
            fixed4 color    : COLOR;
            fixed2 texcoord : TEXCOORD0;
        };  
        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };
    
        // Functions
        void VertexFunction (inout VertexData vertexData, out Input surfaceData)
        {
            float3 worldPosition = mul(unity_ObjectToWorld, vertexData.vertex);
			//fixed3 toCameraDirection = normalize(worldPosition - _WorldSpaceCameraPos);
			//worldPosition += toCameraDirection * ViewDirectionOffset;
            worldPosition += unity_CameraToWorld._m02_m12_m22 * ViewDirectionOffset;
			vertexData.vertex.xyz = mul(unity_WorldToObject, fixed4(worldPosition, 1));

            surfaceData.uv_MainTex = 0;
            surfaceData.color = vertexData.color;
        }
        void SurfaceFunction (Input input, inout SurfaceOutput output)
        {
            fixed4 color = tex2D (_MainTex, input.uv_MainTex) * input.color;
            output.Albedo = PremultipliedAlpha(color);
            output.Alpha = color.a;
        }
        fixed4 LightingLightingFunction (SurfaceOutput surfaceData, half3 directionToLight, half attenuation)
        {
            fixed incidence = 1;
            if(IncidenceMin < 1)
            {
                incidence = dot(directionToLight, surfaceData.Normal);
                incidence = abs(incidence);
                //incidence = (1 + incidence) / 2;   // requires double-sided mesh
                incidence = IncidenceMin + (1 - IncidenceMin) * pow(incidence, IncidenceSharpness);     
            }
            return fixed4(surfaceData.Albedo * _LightColor0.rgb * attenuation * incidence, surfaceData.Alpha);
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}