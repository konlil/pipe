#include "common.inc"
#include "fog.inc"


struct SingleTexture_VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct SingleTexture_VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    
    float3 WorldPosition : TEXCOORD1;  //������
};

SingleTexture_VSOutput SingleTextureVS(SingleTexture_VSInput input)
{
    SingleTexture_VSOutput output;

	float4x4 wvp = mul(mul(World, View), Projection);

    output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;
	
	//��������ռ������
	float4 world_pos = mul(input.Position, World);
	output.WorldPosition = world_pos/world_pos.w;

    return output;
}

float4 SingleTexturePS(SingleTexture_VSOutput input) : COLOR0
{
	float4 diffuse = 0;
	float4 detail = 0;
	float3 final = 0;
	
	//�Է���
	diffuse = float4(MaterialDiffuse, 1.0f);
	if(DiffuseEnabled)
	{
		diffuse = tex2D(SamplerDiffuse, input.TexCoord);
	}

	if(DetailEnabled)
	{	
		detail = tex2D(SamplerDetail, input.TexCoord);
		diffuse.rgb *= detail.rgb;
	}

	//������ & ������
	final += (AmbientColor * diffuse.rgb);
	final += MaterialEmissive;
	
	//if(FogEnabled)
	//	final.rgb = LinearFog(final.rgb, input.WorldPosition);

    return float4(final, diffuse.a);
}

technique TSingleTexture 
<
	string Description = "��ͨ������ͼ";
>
{
	pass p0
	{
		VertexShader = compile vs_1_1 SingleTextureVS();
        PixelShader = compile ps_2_0 SingleTexturePS();
	}
}
