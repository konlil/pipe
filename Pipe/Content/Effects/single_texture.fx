float4x4 World;
float4x4 View;
float4x4 Projection;

texture	DiffuseTexture 
<
	string SasUiLabel = "ÑÕÉ«ÌùÍ¼"; 
	string SasUiControl = "FilePicker";
>;

sampler	SamplerDiffuse = sampler_state
{
	Texture	  =	(DiffuseTexture);
	MipFilter =	LINEAR;
	MinFilter =	LINEAR;
	MagFilter =	LINEAR;
	MipMapLodBias = -2.5f;
};

struct SingleTexture_VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct SingleTexture_VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

SingleTexture_VSOutput SingleTextureVS(SingleTexture_VSInput input)
{
    SingleTexture_VSOutput output;

	float4x4 wvp = mul(mul(World, View), Projection);

    output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;

    return output;
}

float4 SingleTexturePS(SingleTexture_VSOutput input) : COLOR0
{
	float4 final = 0;
	
	final = tex2D(SamplerDiffuse, input.TexCoord);

    return final;
}

technique TSingleTexture 
<
	string Description = "ÆÕÍ¨µ¥²ãÌùÍ¼";
>
{
	pass p0
	{
		VertexShader = compile vs_1_1 SingleTextureVS();
        PixelShader = compile ps_1_1 SingleTexturePS();
	}
}
