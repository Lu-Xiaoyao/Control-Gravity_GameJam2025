Shader "Custom/BackgroundFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed (XY)", Vector) = (0.01, 0.01, 0, 0) // �����ٶ�
        _DistortStrength ("Distort Strength", Range(0, 1)) = 0.1 // Ť��ǿ��
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // ����͸������

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _FlowSpeed;
            float _DistortStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // UV ƫ�ƣ��������ģ�
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + _FlowSpeed * _Time.y; 
                // ������΢�Ŷ���ģ��Ť����
                v.vertex.xy += float2(
                    sin(_Time.y * 2 + v.uv.y * 5) * _DistortStrength, 
                    cos(_Time.y * 3 + v.uv.x * 4) * _DistortStrength
                );
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
