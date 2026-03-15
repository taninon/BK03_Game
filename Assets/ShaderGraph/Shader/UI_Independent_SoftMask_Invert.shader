Shader "Custom/UI_Independent_SoftMask_Invert"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        
        // ★追加：反転フラグ (0: 通常, 1: 反転)
        [MaterialToggle] _InvertMask ("Invert Mask", Float) = 0
        
        // 既存の機能
        _MaskScale ("Mask Scale", Range(0.1, 5.0)) = 1.0
        _MaskBias ("Mask Thickness Bias", Range(-1.0, 1.0)) = 0.0
        
        [HideInInspector] _MaskRect ("Mask Rect (World)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Cull Off Lighting Off ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha 
        ColorMask RGBA

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MaskRect;
            fixed4 _Color;
            float _MaskScale;
            float _MaskBias;
            float _InvertMask; // ★追加

            v2f vert(appdata_t IN) {
                v2f OUT;
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 1. スケール処理付きUV計算
                float2 maskUV = (IN.worldPos.xy - _MaskRect.xy) / (_MaskRect.zw - _MaskRect.xy);
                maskUV = (maskUV - 0.5) * _MaskScale + 0.5;

                float maskValue = 0;
                
                // 範囲内の場合のみサンプリング
                if(maskUV.x >= 0 && maskUV.x <= 1 && maskUV.y >= 0 && maskUV.y <= 1) {
                    maskValue = tex2D(_MaskTex, maskUV).a;
                }

                // ★ここがポイント：反転処理
                // _InvertMaskが1の時だけ、(1.0 - maskValue) を計算する。
                // abs関数を使うことで、0か1かのフラグに応じた計算が1行で書ける。
                // _InvertMask=0なら |0-maskValue|=maskValue, _InvertMask=1なら |1-maskValue|
                float finalMaskAlpha = abs(_InvertMask - maskValue);
                // -----------------------

                // 3. 太さ（バイアス）処理
                finalMaskAlpha = saturate(finalMaskAlpha + _MaskBias);

                color.a *= finalMaskAlpha;
                color.rgb *= color.a; // プリマルチプライド処理

                return color;
            }
        ENDCG
        }
    }
}