// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CJK Custom/Dissolve"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		[Toggle(_UPGRADEPLANE_ON)] _UpgradePlane("Upgrade Plane", Float) = 0
		_DissolveProgress("Dissolve Progress", Range( 0 , 1)) = 0
		_DissolveSmoothness("Dissolve Smoothness", Range( 0 , 1)) = 0.05505115
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _UPGRADEPLANE_ON
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _DissolveProgress;
		uniform float _DissolveSmoothness;
		uniform sampler2D _DissolveTexture;
		uniform float4 _DissolveTexture_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode13 = tex2D( _MainTex, uv_MainTex );
			o.Emission = tex2DNode13.rgb;
			float lerpResult19 = lerp( 1.0 , -1.0 , _DissolveProgress);
			float4 temp_cast_1 = (lerpResult19).xxxx;
			float4 temp_cast_2 = (( lerpResult19 + _DissolveSmoothness )).xxxx;
			float2 uv_DissolveTexture = i.uv_texcoord * _DissolveTexture_ST.xy + _DissolveTexture_ST.zw;
			float2 temp_output_34_0_g1 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break39_g1 = temp_output_34_0_g1;
			float2 appendResult50_g1 = (float2(( 1.0 * ( length( temp_output_34_0_g1 ) * 2.0 ) ) , ( ( atan2( break39_g1.x , break39_g1.y ) * ( 1.0 / 6.28318548202515 ) ) * 1.0 )));
			float2 break53_g1 = appendResult50_g1;
			float4 temp_cast_3 = (break53_g1.y).xxxx;
			#ifdef _UPGRADEPLANE_ON
				float4 staticSwitch21 = temp_cast_3;
			#else
				float4 staticSwitch21 = tex2D( _DissolveTexture, uv_DissolveTexture );
			#endif
			float4 smoothstepResult15 = smoothstep( temp_cast_1 , temp_cast_2 , staticSwitch21);
			o.Alpha = ( tex2DNode13.a * smoothstepResult15 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.RangedFloatNode;16;-1616,576;Inherit;False;Property;_DissolveProgress;Dissolve Progress;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-1248,528;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-1456,80;Inherit;True;Property;_DissolveTexture;Dissolve Texture;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;17;-1376,704;Inherit;False;Property;_DissolveSmoothness;Dissolve Smoothness;5;0;Create;True;0;0;0;False;0;False;0.05505115;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;14;-1408,336;Inherit;False;Polar Coordinates;-1;;1;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;3;FLOAT2;0;FLOAT;55;FLOAT;56
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1040,656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;21;-1104,240;Inherit;False;Property;_UpgradePlane;Upgrade Plane;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-784,16;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SmoothstepOpNode;15;-800,368;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-432,304;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;12;-176,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;CJK Custom/Dissolve;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;2;16;0
WireConnection;18;0;19;0
WireConnection;18;1;17;0
WireConnection;21;1;22;0
WireConnection;21;0;14;56
WireConnection;15;0;21;0
WireConnection;15;1;19;0
WireConnection;15;2;18;0
WireConnection;20;0;13;4
WireConnection;20;1;15;0
WireConnection;12;2;13;0
WireConnection;12;9;20;0
ASEEND*/
//CHKSM=90CF2099AD8656EE0E8236866B7A7C049BB68959