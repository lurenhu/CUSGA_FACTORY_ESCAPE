Shader "Hidden/Fade" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }

        _FadeTex ("FadeTexture", 2D) = "white" { }//溶解的噪声图
        _FadeAmount ("FadeAmount", Range(0, 1)) = 0 //溶解的数值
        _FadeColor ("FadeColor", Color) = (1, 1, 0, 1) //溶解时的颜色
        _FadeBurnWidth ("Fade Burn Width", Range(0, 1)) = 0.02 //当颜色覆盖了多少的时候才开始溶解

    }
    SubShader {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _FadeTex;
            float4 _FadeTex_ST;
            float _FadeAmount, _FadeBurnWidth;
            fixed4 _FadeColor;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);//原始图片采样的颜色
                float originalAlpha = col.a;//原始的alpha值

                float2 tiledUvFade = TRANSFORM_TEX(i.uv, _FadeTex);//得到噪声texture的UV坐标

                float fadeTemp = tex2D(_FadeTex, tiledUvFade).r;//选取噪声texture的r值来进行消失的判定
                float fade = step(_FadeAmount, fadeTemp);//通过texture的r值值来进行溶解判定，高于_FadeAmount为1，反之为0
                float fadeBurn = saturate(step(_FadeAmount - _FadeBurnWidth, fadeTemp));//当颜色覆盖_FadeBurnWidth以后才开始溶解的值 也是0或1
                col.a *= fade;//让原本的颜色的alpha值和fade相乘来表示消失的部分
                col += fadeBurn * tex2D(_FadeTex, tiledUvFade) * _FadeColor * originalAlpha * (1 - col.a);//相乘得到最后的溶解效果

                return col;
            }
            ENDCG
        }
    }
}