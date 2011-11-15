float4x4 World;
float4x4 View;
float4x4 Projection;

// effect parameters.
int Factor2x = 0xCDCDCD;

static float4 m_a : MATERIALAMBIENT = {1.0f, 1.0f, 1.0f, 1.0f};   
static float4 m_d : MATERIALDIFFUSE = {1.0f, 1.0f, 1.0f, 1.0f};    
static float4 m_s : MATERIALSPECULAR = {0.0f, 0.0f, 0.0f, 1.0f};    
static float4 m_e : MATERIALEMISSIVE = {0.0f, 0.0f, 0.0f, 1.0f};
static float power : MATERIALPOWER = 60.0f;                         

texture	Tex0 
<
	string SasUiLabel = "ÑÕÉ«ÌùÍ¼"; 
	string SasUiControl = "FilePicker";
>;

sampler	SamplerDiffuse1 = sampler_state
{
	Texture	  =	(Tex0);
	MipFilter =	LINEAR;
	MinFilter =	LINEAR;
	MagFilter =	LINEAR;
	MipMapLodBias = -2.5f;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

    return float4(1, 0, 0, 1);
}

technique TNoShader
<
	string Description = "ÆÕÍ¨µ¥²ãÌùÍ¼";
>
{
	pass p0
	{
		SpecularEnable = FALSE;
		MaterialAmbient = (m_a);
		MaterialDiffuse = (m_d);
		MaterialEmissive = (m_e);
		MaterialSpecular = (m_s);
		MaterialPower = (power);
		
		AlphaBlendEnable = FALSE;
		AlphaTestEnable = FALSE;

		TextureFactor = (Factor2x);
		
		ColorOp[0] = MODULATE;
		ColorArg1[0] = TEXTURE;
		ColorArg2[0] = TFACTOR;
		AlphaOp[0] = SELECTARG1;
		AlphaArg1[0] = DIFFUSE;
		Sampler[0] = (SamplerDiffuse1);
		TexCoordIndex[0] = 0;
		
		ColorOp[1] = MODULATE2X;
		ColorArg1[1] = CURRENT;
		ColorArg2[1] = DIFFUSE;
		AlphaOp[1] = SELECTARG1;
		AlphaArg1[1] = CURRENT;
		
		ColorOp[2] = DISABLE;
		AlphaOp[2] = DISABLE;
		
		VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
	}
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}
