Shader "FX/Master_UIFX"
{
    Properties
    {
        [Enum(Additive,1,Alpha,10)]_BlendMode("Blend Mode", Float) = 10
        [Toggle(_CUSTOM_DATA_ON)] _Custom_Data("Custom_Data", Float) = 0

        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Main_Intensity("Main_Intensity", Float) = 1
        _Main_Range("Main_Range", Float) = 1

        _Dissolve_Texture("Dissolve Texture", 2D) = "white" {}
        _Dissolve_Progress("Dissolve_Progress", Range( -1 , 1)) = 0

        _Mask_Texture("Mask_Texture", 2D) = "white" {}
        _Mask_Range("Mask_Range", Float) = 1

        [Header(__________________________________________________________________)]
        [Space(17)]

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Fog { Mode Off }
        Blend One [_BlendMode]

        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #pragma shader_feature_local _CUSTOM_DATA_ON
            //커스텀 데이터를 쉐이더 피처로 선언.

            // 버텍스 구조체
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float4 texcoord : TEXCOORD0;
                float4 customData : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // 픽셀 쉐이더로 넘길 때 사용할 그릇?
            struct v2f
            {
                float4 vertex           : SV_POSITION;
                fixed4 color            : COLOR;
                float4 texcoord         : TEXCOORD0;
                float4 worldPosition    : TEXCOORD1;
                float4 customData       : TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Main_Intensity;
            float _Main_Range;

            sampler2D _Dissolve_Texture;
            float4 _Dissolve_Texture_ST;
            float _Dissolve_Progress;

            sampler2D _Mask_Texture;
            float4 _Mask_Texture_ST;
            float _Mask_Range;

            fixed4 _TextureSampleAdd;
            float4 _ClipRect;


            // 버텍스 쉐이더
            v2f vert(appdata_t v) //버텍스 구조체를 가져와서 v2f 함수에 담았다.
            {
                v2f OUT;
                UNITY_INITIALIZE_OUTPUT(v2f, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            // 픽셀 쉐이더
            fixed4 frag(v2f IN) : SV_Target
            {
                #ifdef _CUSTOM_DATA_ON
                float OUT_Dissolve_Progress = IN.texcoord.w;
                #else
                float OUT_Dissolve_Progress = _Dissolve_Progress;
                #endif

                #ifdef _CUSTOM_DATA_ON
                float OUT_Main_Intensity = IN.texcoord.z;
                #else
                float OUT_Main_Intensity = _Main_Intensity;
                #endif

                float mask_default = saturate(tex2D(_Mask_Texture, IN.texcoord.xy * _Mask_Texture_ST.xy + _Mask_Texture_ST.zw).a);
                float mask_alpha = saturate(pow(mask_default,_Mask_Range));
                // 첫번째 매개변수엔 텍스처 샘플러를 담아주고, 두번째 매개변수엔 UV 정보를 담아줌.
                // ST_xy값을 곱하면 타일링, ST_zw값을 더하면 오프셋이 됨
                // r값만 쓰는 이유는 차피 mask 텍스처들은 float1으로 이루어져 있기 때문.

                float dissolve_alpha = saturate(tex2D(_Dissolve_Texture, IN.texcoord.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw).a + OUT_Dissolve_Progress);
                // IN.texcoord.xyzw
                // xy -> UV값
                // _ST.xy를 곱해주고, zw를 더해주는 것은 스케일, 타일링 정보를 넣는 것.
                // dissolve 안엔 디졸브 텍스처 정보가 들어 있음
                // IN.texcoord.z을 더해서 커스텀 데이터로 사용하는 UV값을 더해줌.

                float4 noise = saturate(mask_alpha * dissolve_alpha);

                float4 main_color = (tex2D(_MainTex, IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw) + _TextureSampleAdd) * IN.color;
                float4 Out_color = saturate(pow(main_color,_Main_Range));

                Out_color.a *= noise;

                Out_color.rgb *= OUT_Main_Intensity;

                #ifdef UNITY_UI_CLIP_RECT
                    Out_color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip (Out_color.a - 0.001);
                #endif

                Out_color.rgb *= Out_color.a;

                return Out_color;
            }
            ENDCG
        }
    }
    Fallback Off
}
