#if OPENGL
	#define PS_SHADERMODEL ps_3_0
#else
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

float4 MainPS(float2 coords : TEXCOORD0) : COLOR0
{
    return tex2D(s0, coords);
}

technique T0
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};