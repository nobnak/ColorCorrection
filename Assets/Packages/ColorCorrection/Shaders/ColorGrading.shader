Shader "Hidden/ColorGrading" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

            CGINCLUDE
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;

            float _ColorGrading_Scale;
            float _ColorGrading_Offset;
            sampler3D _ColorGrading_3DLut;

            float4 frag (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);
                c.rgb = tex3D(_ColorGrading_3DLut, c.rgb * _ColorGrading_Scale + _ColorGrading_Offset).rgb;
                return c;
            }
            float4 fragApproxLinear (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);
                c.rgb = sqrt(c.rgb);
                c.rgb = tex3D(_ColorGrading_3DLut, c.rgb * _ColorGrading_Scale + _ColorGrading_Offset).rgb;
                c.rgb *= c.rgb;
                return c;
            }
            ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragApproxLinear
            ENDCG
        }
	}
}
