Shader "Custom/UI_Independent_SoftMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        // C#から送られてくる値（表示には出ない）
        _MaskRect ("Mask Rect (World)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "CanUseSpriteAtlas"="True"
        }

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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1; // ワールド座標保持用
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MaskRect;
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                // オブジェクトの頂点をワールド座標に変換
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. 本体の色
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 2. 現在のピクセルが「マスクの範囲」のどこにいるか(0.0〜1.0)を計算
                // (現在位置 - 最小値) / (最大値 - 最小値)
                float2 maskUV = (IN.worldPos.xy - _MaskRect.xy) / (_MaskRect.zw - _MaskRect.xy);

                // 3. マスク範囲外ならアルファを0にする
                float maskAlpha = 0;
                if(maskUV.x >= 0 && maskUV.x <= 1 && maskUV.y >= 0 && maskUV.y <= 1)
                {
                    maskAlpha = tex2D(_MaskTex, maskUV).a;
                }

                // 4. 合成
                color.a *= maskAlpha;
                color.rgb *= color.a; // プリマルチプライド処理

                return color;
            }
        ENDCG
        }
    }
}