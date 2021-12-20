Shader "Custom/Outline" {
    
    Properties{
        _Color("Ouline Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Thickness("Outline Thickness", Float) = 0.1
    }

    SubShader{

        Tags { "Queue" = "Transparent" }

        Pass {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            ZWrite Off

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert(float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            float4 frag(void) : COLOR {
                return float4(0.0, 0.0, 0.0, 0.0);
            }
            ENDCG
        }

        Pass {
            Cull Off
            Stencil {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _Color;
            uniform float _Thickness;

            float4 vert(float4 vertex : POSITION, float3 normal : NORMAL) : SV_POSITION
            {
                float depth = -UnityObjectToViewPos(vertex).z;
                return UnityObjectToClipPos(vertex + normal * _Thickness * sqrt(depth));
            }

            float4 frag(void) : COLOR 
            {
                return _Color;
            }
            ENDCG
        }
    }
}