#include "common.inc"
#include "fog.inc"
#include "light.inc"

///////////////////////////////////////////////

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	    
    float3 WorldPosition : TEXCOORD1;  //用于雾化
    float3 WorldNormal : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 wvp = mul(mul(World, View), Projection);
	
	output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;
	
	float4 world_pos = mul(input.Position, World);
	output.WorldPosition = world_pos/world_pos.w;
	
	output.WorldNormal = mul(input.Normal, World);	

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 diffuse = 0;
	float4 detail = 0;
	float3 final = 0;
	
	//自发光
	diffuse = float4(MaterialDiffuse, 1.0f);
	if(DiffuseEnabled)
	{
		diffuse = tex2D(SamplerDiffuse, input.TexCoord*DiffuseUVTile); //
	}
	
	if(DetailEnabled)
	{	
		detail = tex2D(SamplerDetail, input.TexCoord*DetailUVTile);
		diffuse.rgb *= detail.rgb;
	}
	
	//灯光的影响
	float3 normal = normalize(input.WorldNormal);
	for(int i=0; i < MAX_TOTAL_LIGHTS; i++)
	{
		if( Lights[i].enabled )
			final += CalcSingleLight( Lights[i], input.WorldPosition, normal, diffuse, MaterialSpecular, MaterialSpecPower);
	}
	
	//环境光 & 漫反射
	final += (AmbientColor * diffuse.rgb);
	final += MaterialEmissive;
	
	if(FogEnabled)
		final.rgb = LinearFog(final.rgb, input.WorldPosition);

    return float4(final, diffuse.a);
}

technique TGeneric
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
