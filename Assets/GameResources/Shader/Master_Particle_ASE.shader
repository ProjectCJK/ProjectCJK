// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FX/Master_Particle"
{
	Properties
	{
		[Toggle(_CUSTOMDATA_ON)] _CustomData("CustomData", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)]_SRC("SRC", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_DST("DST", Float) = 10
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Main_Color("Main_Color", Color) = (1,1,1,1)
		_Main_Intensity("Main_Intensity", Float) = 1
		_Main_Power("Main_Power", Range( 1 , 10)) = 1
		_Main_UOffset("Main_UOffset", Float) = 0
		_Main_VOffset("Main_VOffset", Float) = 0
		_Main_UPanner("Main_UPanner", Float) = 0
		_Main_VPanner("Main_VPanner", Float) = 0
		_Dissolve_Texture("Dissolve_Texture", 2D) = "white" {}
		_Dissolve_Progress("Dissolve_Progress", Range( -1 , 1)) = 1
		_Dissolve_UOffset("Dissolve_UOffset", Float) = 0
		_Dissolve_VOffset("Dissolve_VOffset", Float) = 0
		_Mask_Texture("Mask_Texture", 2D) = "white" {}
		_Mask_Power("Mask_Power", Range( 1 , 10)) = 1
		_Mask_Range("Mask_Range", Range( 1 , 10)) = 1
		_Noise_Texture("Noise_Texture", 2D) = "bump" {}
		_Noise_Scale("Noise_Scale", Range( 0 , 1)) = 0
		_Noise_UOffset("Noise_UOffset", Float) = 0
		_Noise_VOffset("Noise_VOffset", Float) = 0
		_Noise_UPanner("Noise_UPanner", Float) = 0
		_Noise_VPanner("Noise_VPanner", Float) = 0
		_SecondColor("Second Color", Color) = (1,1,1,0)
		_SecondColorProgress("Second Color Progress", Range( 0 , 1)) = 0
		[Toggle(_RAMPONOFF_ON)] _RampOnOff("Ramp OnOff", Float) = 0
		_RampTexture("RampTexture", 2D) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite Off
		Blend [_SRC] [_DST]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _RAMPONOFF_ON
		#pragma shader_feature_local _CUSTOMDATA_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_texcoord;
			float4 uv3_texcoord3;
			float4 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

		uniform float _SRC;
		uniform float _DST;
		uniform float _Cull;
		uniform float4 _Main_Color;
		uniform sampler2D _MainTex;
		uniform float _Main_UPanner;
		uniform float _Main_VPanner;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Noise_Texture;
		uniform float _Noise_UPanner;
		uniform float _Noise_VPanner;
		uniform float4 _Noise_Texture_ST;
		uniform float _Noise_UOffset;
		uniform float _Noise_VOffset;
		uniform float _Noise_Scale;
		uniform float _Main_UOffset;
		uniform float _Main_VOffset;
		uniform sampler2D _RampTexture;
		uniform float _Main_Intensity;
		uniform float4 _SecondColor;
		uniform float _SecondColorProgress;
		uniform float _Main_Power;
		uniform sampler2D _Mask_Texture;
		uniform float4 _Mask_Texture_ST;
		uniform float _Mask_Range;
		uniform float _Mask_Power;
		uniform sampler2D _Dissolve_Texture;
		uniform float4 _Dissolve_Texture_ST;
		uniform float _Dissolve_UOffset;
		uniform float _Dissolve_VOffset;
		uniform float _Dissolve_Progress;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult158 = (float2(_Main_UPanner , _Main_VPanner));
			float4 uvs_MainTex = i.uv_texcoord;
			uvs_MainTex.xy = i.uv_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner159 = ( 1.0 * _Time.y * appendResult158 + uvs_MainTex.xy);
			float2 appendResult150 = (float2(_Noise_UPanner , _Noise_VPanner));
			float4 uvs_Noise_Texture = i.uv_texcoord;
			uvs_Noise_Texture.xy = i.uv_texcoord.xy * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
			float2 panner146 = ( 1.0 * _Time.y * appendResult150 + uvs_Noise_Texture.xy);
			#ifdef _CUSTOMDATA_ON
				float staticSwitch141 = i.uv3_texcoord3.x;
			#else
				float staticSwitch141 = _Noise_UOffset;
			#endif
			#ifdef _CUSTOMDATA_ON
				float staticSwitch140 = i.uv3_texcoord3.y;
			#else
				float staticSwitch140 = _Noise_VOffset;
			#endif
			float2 appendResult142 = (float2(staticSwitch141 , staticSwitch140));
			#ifdef _CUSTOMDATA_ON
				float staticSwitch95 = i.uv2_texcoord2.z;
			#else
				float staticSwitch95 = _Main_UOffset;
			#endif
			#ifdef _CUSTOMDATA_ON
				float staticSwitch92 = i.uv2_texcoord2.w;
			#else
				float staticSwitch92 = _Main_VOffset;
			#endif
			float2 appendResult93 = (float2(staticSwitch95 , staticSwitch92));
			float4 tex2DNode25 = tex2D( _MainTex, (( float3( panner159 ,  0.0 ) + UnpackScaleNormal( tex2D( _Noise_Texture, (panner146*1.0 + appendResult142) ), _Noise_Scale ) )*1.0 + float3( appendResult93 ,  0.0 )).xy );
			float2 appendResult175 = (float2(tex2DNode25.r , 1.0));
			#ifdef _RAMPONOFF_ON
				float3 staticSwitch179 = tex2D( _RampTexture, appendResult175 ).rgb;
			#else
				float3 staticSwitch179 = tex2DNode25.rgb;
			#endif
			#ifdef _CUSTOMDATA_ON
				float staticSwitch80 = i.uv_texcoord.z;
			#else
				float staticSwitch80 = _Main_Intensity;
			#endif
			#ifdef _CUSTOMDATA_ON
				float staticSwitch171 = i.uv3_texcoord3.x;
			#else
				float staticSwitch171 = _SecondColorProgress;
			#endif
			float4 lerpResult167 = lerp( ( _Main_Color * float4( ( staticSwitch179 * staticSwitch80 ) , 0.0 ) ) , _SecondColor , staticSwitch171);
			o.Emission = ( lerpResult167 * i.vertexColor ).rgb;
			float4 uvs_Mask_Texture = i.uv_texcoord;
			uvs_Mask_Texture.xy = i.uv_texcoord.xy * _Mask_Texture_ST.xy + _Mask_Texture_ST.zw;
			float4 tex2DNode50 = tex2D( _Mask_Texture, uvs_Mask_Texture.xy );
			float saferPower53 = abs( saturate( ( ( tex2DNode50.r * tex2DNode50.a ) * _Mask_Range ) ) );
			float4 uvs_Dissolve_Texture = i.uv_texcoord;
			uvs_Dissolve_Texture.xy = i.uv_texcoord.xy * _Dissolve_Texture_ST.xy + _Dissolve_Texture_ST.zw;
			#ifdef _CUSTOMDATA_ON
				float staticSwitch74 = i.uv2_texcoord2.x;
			#else
				float staticSwitch74 = _Dissolve_UOffset;
			#endif
			#ifdef _CUSTOMDATA_ON
				float staticSwitch75 = i.uv2_texcoord2.y;
			#else
				float staticSwitch75 = _Dissolve_VOffset;
			#endif
			float2 appendResult67 = (float2(staticSwitch74 , staticSwitch75));
			float4 tex2DNode62 = tex2D( _Dissolve_Texture, (uvs_Dissolve_Texture.xy*1.0 + appendResult67) );
			#ifdef _CUSTOMDATA_ON
				float staticSwitch83 = i.uv_texcoord.w;
			#else
				float staticSwitch83 = _Dissolve_Progress;
			#endif
			o.Alpha = saturate( ( i.vertexColor.a * ( ( saturate( pow( tex2DNode25.a , _Main_Power ) ) * saturate( ( saturate( pow( saferPower53 , _Mask_Power ) ) * saturate( ( ( tex2DNode62.r * tex2DNode62.a ) + staticSwitch83 ) ) ) ) ) * _Opacity ) ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.CommentaryNode;137;-5440,-1184;Inherit;False;1617.797;1015.987;Comment;15;133;134;141;138;139;140;142;143;144;145;146;148;150;151;152;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;138;-5280,-736;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;139;-5280,-400;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;143;-5264,-832;Inherit;False;Property;_Noise_UOffset;Noise_UOffset;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-5280,-544;Inherit;False;Property;_Noise_VOffset;Noise_VOffset;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-5136,-1040;Inherit;False;Property;_Noise_UPanner;Noise_UPanner;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-5136,-944;Inherit;False;Property;_Noise_VPanner;Noise_VPanner;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;87;-2960.512,896;Inherit;False;2595.373;1002.398;Dissolve;16;64;71;67;75;74;69;68;118;83;85;62;84;132;86;79;78;Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;140;-4960,-544;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;141;-4960,-704;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;90;-3711.993,-1278;Inherit;False;3550.426;1477.389;Main;29;100;101;44;80;81;110;45;111;25;91;153;93;159;95;92;155;158;97;96;98;99;157;156;176;174;172;173;175;179;Main;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;148;-4912,-1104;Inherit;False;0;133;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;150;-4848,-960;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;89;-1565.6,414;Inherit;False;1391.6;380.2275;Mask;9;53;54;52;50;104;51;108;160;161;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2928,1040;Inherit;False;Property;_Dissolve_UOffset;Dissolve_UOffset;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-2928,1360;Inherit;False;Property;_Dissolve_VOffset;Dissolve_VOffset;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;79;-2944,1472;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;78;-2944,1152;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;142;-4672,-656;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;146;-4640,-960;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-3664,-864;Inherit;False;Property;_Main_UPanner;Main_UPanner;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;157;-3664,-768;Inherit;False;Property;_Main_VPanner;Main_VPanner;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-1520,480;Inherit;False;0;50;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;74;-2640,1136;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;True;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;75;-2656,1344;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;-4464,-672;Inherit;False;Property;_Noise_Scale;Noise_Scale;21;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;145;-4400,-896;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;99;-3584,-480;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;98;-3584,-144;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;96;-3568,-576;Inherit;False;Property;_Main_UOffset;Main_UOffset;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-3584,-288;Inherit;False;Property;_Main_VOffset;Main_VOffset;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;158;-3376,-784;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;155;-3456,-960;Inherit;False;0;25;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;50;-1296,480;Inherit;True;Property;_Mask_Texture;Mask_Texture;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DynamicAppendNode;67;-2320,1232;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;71;-2464,976;Inherit;False;0;62;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;133;-4112,-784;Inherit;True;Property;_Noise_Texture;Noise_Texture;20;0;Create;True;0;0;0;False;0;False;-1;None;c0439e6cfa9603d40bf4a113220c5833;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StaticSwitch;92;-3264,-288;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;95;-3264,-448;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;159;-3168,-784;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;108;-976,496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-1184,688;Inherit;False;Property;_Mask_Range;Mask_Range;19;0;Create;True;0;0;0;False;0;False;1;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;64;-2112,1184;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;93;-2944,-400;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;153;-2976,-736;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-784,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-1664,1392;Inherit;False;Property;_Dissolve_Progress;Dissolve_Progress;14;0;Create;True;0;0;0;False;0;False;1;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;62;-1824,1120;Inherit;True;Property;_Dissolve_Texture;Dissolve_Texture;13;0;Create;True;0;0;0;False;0;False;-1;None;a59dfa6f994e9bf48852893951a53476;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexCoordVertexDataNode;85;-1616,1520;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;91;-2704,-784;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;160;-608,528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-784,672;Inherit;False;Property;_Mask_Power;Mask_Power;18;0;Create;True;0;0;0;False;0;False;1;0;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;83;-1360,1488;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1520,1184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-2432,-880;Inherit;True;Property;_MainTex;MainTex;5;0;Create;True;0;0;0;False;0;False;-1;None;5ab1271cc89045649bcce615c4b929e9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.PowerNode;53;-448,528;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;132;-1040,1328;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-1744,-752;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;173;-1808,-976;Inherit;True;True;False;False;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;161;-304,544;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;86;-768,1312;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-1136,0;Inherit;False;Property;_Main_Power;Main_Power;8;0;Create;True;0;0;0;False;0;False;1;0;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;175;-1520,-880;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;174;-1600,-1168;Inherit;True;Property;_RampTexture;RampTexture;30;0;Create;True;0;0;0;False;0;False;887765d0c42abe44c9650b3f673f3e04;887765d0c42abe44c9650b3f673f3e04;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-144,544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1088,-560;Inherit;False;Property;_Main_Intensity;Main_Intensity;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;81;-1088,-448;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;110;-800,-96;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;176;-1232,-1040;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode;164;16,400;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;165;16,560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;80;-832,-496;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;179;-832,-720;Inherit;False;Property;_RampOnOff;Ramp OnOff;29;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;56;96,800;Inherit;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;208,496;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-112,0;Inherit;False;Property;_SecondColorProgress;Second Color Progress;28;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;170;-32,160;Inherit;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-512,-544;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;101;-656,-1072;Inherit;False;Property;_Main_Color;Main_Color;6;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;432,528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;58;560,160;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;171;240,80;Inherit;False;Property;_CustomData;CustomData;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;74;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;166;288,-128;Inherit;False;Property;_SecondColor;Second Color;27;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-304,-560;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;928,480;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;167;688,-80;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;1632,240;Inherit;False;Property;_SRC;SRC;2;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;1648,336;Inherit;False;Property;_DST;DST;3;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;1648,144;Inherit;False;Property;_Cull;Cull;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;1040,128;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;61;1104,464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;162;1344,240;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;FX/Master_Particle;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;True;_SRC;10;True;_DST;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;26;-1;-1;-1;0;False;0;0;True;_Cull;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;140;1;144;0
WireConnection;140;0;139;2
WireConnection;141;1;143;0
WireConnection;141;0;138;1
WireConnection;150;0;151;0
WireConnection;150;1;152;0
WireConnection;142;0;141;0
WireConnection;142;1;140;0
WireConnection;146;0;148;0
WireConnection;146;2;150;0
WireConnection;74;1;68;0
WireConnection;74;0;78;1
WireConnection;75;1;69;0
WireConnection;75;0;79;2
WireConnection;145;0;146;0
WireConnection;145;2;142;0
WireConnection;158;0;156;0
WireConnection;158;1;157;0
WireConnection;50;1;51;0
WireConnection;67;0;74;0
WireConnection;67;1;75;0
WireConnection;133;1;145;0
WireConnection;133;5;134;0
WireConnection;92;1;97;0
WireConnection;92;0;98;4
WireConnection;95;1;96;0
WireConnection;95;0;99;3
WireConnection;159;0;155;0
WireConnection;159;2;158;0
WireConnection;108;0;50;1
WireConnection;108;1;50;4
WireConnection;64;0;71;0
WireConnection;64;2;67;0
WireConnection;93;0;95;0
WireConnection;93;1;92;0
WireConnection;153;0;159;0
WireConnection;153;1;133;0
WireConnection;52;0;108;0
WireConnection;52;1;104;0
WireConnection;62;1;64;0
WireConnection;91;0;153;0
WireConnection;91;2;93;0
WireConnection;160;0;52;0
WireConnection;83;1;84;0
WireConnection;83;0;85;4
WireConnection;118;0;62;1
WireConnection;118;1;62;4
WireConnection;25;1;91;0
WireConnection;53;0;160;0
WireConnection;53;1;54;0
WireConnection;132;0;118;0
WireConnection;132;1;83;0
WireConnection;173;0;25;1
WireConnection;161;0;53;0
WireConnection;86;0;132;0
WireConnection;175;0;173;0
WireConnection;175;1;172;0
WireConnection;88;0;161;0
WireConnection;88;1;86;0
WireConnection;110;0;25;4
WireConnection;110;1;111;0
WireConnection;176;0;174;0
WireConnection;176;1;175;0
WireConnection;164;0;110;0
WireConnection;165;0;88;0
WireConnection;80;1;45;0
WireConnection;80;0;81;3
WireConnection;179;1;25;5
WireConnection;179;0;176;5
WireConnection;105;0;164;0
WireConnection;105;1;165;0
WireConnection;44;0;179;0
WireConnection;44;1;80;0
WireConnection;55;0;105;0
WireConnection;55;1;56;0
WireConnection;171;1;168;0
WireConnection;171;0;170;1
WireConnection;100;0;101;0
WireConnection;100;1;44;0
WireConnection;60;0;58;4
WireConnection;60;1;55;0
WireConnection;167;0;100;0
WireConnection;167;1;166;0
WireConnection;167;2;171;0
WireConnection;59;0;167;0
WireConnection;59;1;58;0
WireConnection;61;0;60;0
WireConnection;162;2;59;0
WireConnection;162;9;61;0
ASEEND*/
//CHKSM=81BF8271AE31E9DA6085859C31A1295F9B4A9A07