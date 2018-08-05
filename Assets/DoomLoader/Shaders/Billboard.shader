Shader "Doom/Billboard" 
{
	Properties
	{
		[PerRenderData]_MainTex("Texture Image", 2D) = "white" {}
		[PerRenderData]_ScaleX("Scale X", Float) = 1.0
		[PerRenderData]_ScaleY("Scale Y", Float) = 1.0
		[PerRenderData]_Brightness("Brightness", Float) = 1.0
	}
		
	SubShader
	{
		Tags
		{
			"DisableBatching" = "True"
		}
		
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform half4 _AMBIENTLIGHT = half4(1, 1, 1, 1);

			uniform sampler2D _MainTex;
			uniform float _ScaleX;
			uniform float _ScaleY;
			uniform float _Brightness;

			struct vertexInput 
			{
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
			};
	
			struct vertexOutput 
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				//rotate to face camera
				output.pos = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					+ float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
					* float4(_ScaleX, _ScaleY, 1.0, 1.0));

				output.tex = input.tex;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float4 color = tex2D(_MainTex, float2(input.tex.xy));
				
				if (color.a < .5f)
					discard;
				
				color *= _AMBIENTLIGHT * _Brightness;

				return color;
			}

			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }

			Blend One One
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag
			#pragma multi_compile_fwdadd

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			uniform sampler2D _MainTex;
			uniform float _ScaleX;
			uniform float _ScaleY;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				LIGHTING_COORDS(1, 2)
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.pos = mul(UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					+ float4(v.vertex.x, v.vertex.y, 0.0, 0.0)
					* float4(_ScaleX, _ScaleY, 1.0, 1.0));

				o.tex = v.tex;

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				float4 color = tex2D(_MainTex, float2(input.tex.xy));

				if (color.a < .5f)
					discard;

				float atten = LIGHT_ATTENUATION(input);

				color.rgb *= atten * atten * _LightColor0.rgb;

				return color;
			}

			ENDCG
		}
	}
}