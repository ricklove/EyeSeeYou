Shader "EyeShader"
{
	Properties 
	{
_Color("Main Color", Color) = (1,1,1,1)
_SpecularColor("Spec Color", Color) = (1,1,1,1)
_Shine("Shine", Range(0.2,1.5) ) = 0.2
_IllumPower("Iris Illum", Range(-0.8,0.5) ) = 0
_ReflectionPower("Reflect Power", Range(0,2) ) = 1.68
_MainTex("RGB Tex (A)Illum", 2D) = "white" {}
_NormalTex("Normal", 2D) = "bump" {}
_Reflect("Reflect Cube", Cube) = "gray" {}

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
//#pragma exclude_renderers d3d11 d3d11_9x
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0



float4 _Color;
float4 _SpecularColor;
float _Shine;
float _IllumPower;
float _ReflectionPower;
sampler2D _MainTex;
sampler2D _NormalTex;
samplerCUBE _Reflect;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float3 simpleWorldRefl;
float2 uv_MainTex;
float2 uv_NormalTex;

			};

			void vert (inout appdata_full v, out Input o) {
UNITY_INITIALIZE_OUTPUT(Input,o);
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

o.simpleWorldRefl = -reflect( normalize(WorldSpaceViewDir(v.vertex)), normalize(mul((float3x3)_Object2World, SCALED_NORMAL)));

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 TexCUBE0=texCUBE(_Reflect,float4( IN.simpleWorldRefl.x, IN.simpleWorldRefl.y,IN.simpleWorldRefl.z,1.0 ));
float4 Multiply3=TexCUBE0 * _ReflectionPower.xxxx;
float4 Tex2D0=tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
float4 Add0=Multiply3 + Tex2D0;
float4 Multiply0=_Color * Add0;
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_NormalTex,(IN.uv_NormalTex.xyxy).xy)).xyz, 1.0 );
float4 Add1=Tex2D0 + _IllumPower.xxxx;
float4 Multiply1=Tex2D0.aaaa * Add1;
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply0;
o.Normal = Tex2DNormal0;
o.Emission = Multiply1;
o.Specular = _Shine.xxxx;
o.Gloss = _SpecularColor;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}