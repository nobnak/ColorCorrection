// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ColorGrading/ColorGrading3D" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}

	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

        CGINCLUDE
            #define LUT3D
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _Lut;

            float4 frag (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);

                #ifdef UNITY_COLORSPACE_GAMMA
                c.rgb = GammaToLinear(c.rgb);
                c = ColorGrade3D(c);
                c.rgb = LinearToGammaSpace(c.rgb);
                #else
                c = ColorGrade3D(c);
                #endif

                return c;
            }
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
