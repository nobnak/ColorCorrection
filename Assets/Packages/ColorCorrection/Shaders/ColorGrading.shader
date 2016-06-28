Shader "ColorGrading/ColorGrading" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
        _Lut ("LUT", 2D) = "black" {}

	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

            CGINCLUDE
            //#define LUT3D
            #include "UnityCG.cginc" 
            #include "Assets/Packages/ColorCorrection/Shaders/LUT.cginc"

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
            sampler2D _Lut;

            float4 frag (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);
                return ColorGrade(_Lut, c);
            }
            float4 fragApproxLinear (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);
                c.rgb = sqrt(c.rgb);
                c = ColorGrade(_Lut, c);
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
