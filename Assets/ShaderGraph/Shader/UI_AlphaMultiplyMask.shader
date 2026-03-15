Shader "Custom/UI_AlphaMultiplyMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // ★マスクとして使うテクスチャ
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        
        // 境界を白く保つためのプリマルチプライドアルファ設定
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
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _MaskTex;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. 本体の色とアルファを取得
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;

                // 2. マスクのアルファを取得（同じUVを使用）
                half maskAlpha = tex2D(_MaskTex, IN.texcoord).a;

                // 3. アルファを掛け合わせる（ここで「重なった部分」が決まる）
                color.a *= maskAlpha;

                // 4. プリマルチプライドアルファの処理（境界線を綺麗にする）
                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
}