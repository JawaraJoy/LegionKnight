Shader "UI/InvertMaskRoundedRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HoleRect ("Hole Rect (x, y, w, h)", Vector) = (0.5, 0.5, 0.3, 0.2)
        _HoleRadius ("Hole Corner Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _HoleRect; // x, y, w, h (center, size) in normalized [0,1]
            float _HoleRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            // Signed distance to a rounded rectangle
            float sdRoundRect(float2 p, float2 b, float r)
            {
                float2 d = abs(p) - b + r;
                return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - r;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.texcoord;
                float2 rectCenter = _HoleRect.xy;
                float2 rectSize = _HoleRect.zw * 0.5;
                float radius = _HoleRadius;

                float2 local = uv - rectCenter;
                float dist = sdRoundRect(local, rectSize, radius);

                if (dist < 0)
                {
                    // Inside the rounded rect hole: transparent
                    return fixed4(0,0,0,0);
                }
                return tex2D(_MainTex, uv) * i.color;
            }
            ENDCG
        }
    }
}
