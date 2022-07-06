Shader "SupGames/Mobile/PostProcess"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

		CGINCLUDE

#include "UnityCG.cginc"
		struct appdata {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	struct v2fb {
		half4 pos : POSITION;
		half4 uv : TEXCOORD0;
	};
	struct v2f {
		half4 pos : POSITION;
		half4 uv  : TEXCOORD0;
		half3 data  : TEXCOORD1;
	};

	sampler2D _MainTex;
	sampler2D _LutTex2D;
	sampler3D _LutTex3D;
	sampler2D _MaskTex;
	sampler2D _BlurTex;
	half _LutAmount;
	half _BloomThreshold;
	half _BloomAmount;
	half _BlurAmount;
	half _LutDimension;
	half _Contrast;
	half _Brightness;
	half _Saturation;
	half _Exposure;
	half _Gamma;
	half _Offset;
	half _Vignette;
	half4 _MainTex_TexelSize;

	v2fb vertBlur(appdata i)
	{
		v2fb o;
		o.pos = UnityObjectToClipPos(i.pos);
		half2 offset = _MainTex_TexelSize.xy * _BlurAmount;
		o.uv = half4(i.uv - offset, i.uv + offset);
		return o;
	}

	v2f vert(appdata i)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.uv.xy = i.uv;
		o.uv.zw = i.uv - 0.5h;
		o.data.x = i.uv.x - _Offset * _MainTex_TexelSize.x;
		o.data.y = i.uv.x + _Offset * _MainTex_TexelSize.x;
		o.data.z = i.uv.y - _Offset * _MainTex_TexelSize.y;
		return o;
	}

	fixed4 fragBloom(v2fb i) : COLOR
	{
		fixed4 result = tex2D(_MainTex, i.uv.xy);
		result += tex2D(_MainTex, i.uv.xw);
		result += tex2D(_MainTex, i.uv.zy);
		result += tex2D(_MainTex, i.uv.zw);
		return max(result * 0.25h - _BloomThreshold, 0.0h);
	}

		fixed4 fragBlur(v2fb i) : COLOR
	{
		fixed4 result = tex2D(_MainTex, i.uv.xy);
		result += tex2D(_MainTex, i.uv.xw);
		result += tex2D(_MainTex, i.uv.zy);
		result += tex2D(_MainTex, i.uv.zw);
		return result * 0.25h;
	}

		fixed4 fragAll2D(v2f i) : COLOR
	{
		fixed4 c;
		fixed bx;
		fixed by;

#if !defined(CHROMA)
		c = tex2D(_MainTex, i.uv);
#else   
		c.r = tex2D(_MainTex, fixed2(i.data.r, i.uv.y)).r;
		c.g = tex2D(_MainTex, fixed2(i.data.g, i.uv.y)).g;
		c.b = tex2D(_MainTex, fixed2(i.uv.x, i.data.b)).b;
		c.a = 1.0h;
#endif

#if defined(BLUR) || defined(BLOOM)
		fixed4 b = tex2D(_BlurTex, i.uv);
#endif

#if defined(BLUR) && !defined(BLOOM)
		fixed4 m = tex2D(_MaskTex, i.uv);
#endif

#if defined(LUT)
		bx = floor(c.b * 256.0h);
		by = floor(bx * 0.0625h);
		c = lerp(c, tex2D(_LutTex2D, c.rg * 0.05859375h + 0.001953125h + fixed2(floor(bx - by * 16.0h), by) * 0.0625h), _LutAmount);
#endif

#if defined(LUT) && (defined(BLOOM) || defined(BLUR))
		bx = floor(b.b * 256.0h);
		by = floor(bx * 0.0625h);
		b = lerp(b, tex2D(_LutTex2D, b.rg * 0.05859375h + 0.001953125h + fixed2(floor(bx - by * 16.0h), by) * 0.0625h), _LutAmount);
#endif

#if defined(BLOOM)
		c = (c + b * _BloomAmount) * 0.5h;
#elif defined(BLUR)
		c = lerp(c, b, m.r);
#endif

#if defined(FILTER)
		c.rgb = (c.rgb - 0.5f) * _Contrast + _Brightness;
		c.rgb = lerp(dot(c.rgb, fixed3(0.299h, 0.587h, 0.114h)), c.rgb, _Saturation);
		c.rgb *= pow(2, _Exposure) - _Gamma;
#endif
		c.rgb *= 1.0h - dot(i.uv.zw, i.uv.zw) * _Vignette;
		return c;
	}


		fixed4 fragAll3D(v2f i) : COLOR
	{
		fixed4 c;

#if !defined(CHROMA)
		c = tex2D(_MainTex, i.uv);
#else   
		c.r = tex2D(_MainTex, fixed2(i.uv.x - _Offset * _MainTex_TexelSize.x, i.uv.y)).r;
		c.g = tex2D(_MainTex, fixed2(i.uv.x + _Offset * _MainTex_TexelSize.x, i.uv.y)).g;
		c.b = tex2D(_MainTex, fixed2(i.uv.x, i.uv.y - _Offset * _MainTex_TexelSize.y)).b;
		c.a = 1.0h;
#endif

#if defined(BLUR) || defined(BLOOM)
		fixed4 b = tex2D(_BlurTex, i.uv);
#endif

#if defined(BLUR) && !defined(BLOOM)
		fixed4 m = tex2D(_MaskTex, i.uv);
#endif

#if defined(LUT)
		c = lerp(c, tex3D(_LutTex3D, c.rgb * 0.9375h + 0.03125h), _LutAmount);
#endif

#if defined(LUT) && (defined(BLOOM)|| defined(BLUR))
		b = lerp(b, tex3D(_LutTex3D, b.rgb * 0.9375h + 0.03125h), _LutAmount);
#endif

#if defined(BLOOM)
		c = (c + b * _BloomAmount) * 0.5h;
#elif defined(BLUR)
		c = lerp(c, b, m.r);
#endif

#if defined(FILTER)
		c.rgb = (c.rgb - 0.5f) * _Contrast + _Brightness;
		c.rgb = lerp(dot(c.rgb, fixed3(0.299h, 0.587h, 0.114h)), c.rgb, _Saturation);
		c.rgb *= pow(2, _Exposure) - _Gamma;
#endif
		c.rgb *= 1.0h - dot(i.uv.zw, i.uv.zw) * _Vignette;
		return c;
	}

		ENDCG

		Subshader
	{
		Pass //0
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBlur
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
			Pass //1
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBloom
		  #pragma fragmentoption ARB_precision_hint_fastest
		  ENDCG
		}
			Pass //2
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment fragAll2D
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma shader_feature BLOOM
		  #pragma shader_feature BLUR
		  #pragma shader_feature CHROMA
		  #pragma shader_feature LUT
		  #pragma shader_feature FILTER
		  ENDCG
		}
			Pass //3
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment fragAll3D
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma shader_feature BLOOM
		  #pragma shader_feature BLUR
		  #pragma shader_feature CHROMA
		  #pragma shader_feature LUT
		  #pragma shader_feature FILTER
		  ENDCG
		}
	}
	Fallback off
}