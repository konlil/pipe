
shared uniform extern float3 AmbientColor = float3(0.1, 0.1, 0.1);

uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;

//----------------------------------------------------
//	材质参数
//----------------------------------------------------
uniform extern float3	MaterialDiffuse;						//	材质漫反射颜色
uniform extern float3	MaterialSpecular;						//	材质镜面高光颜色
uniform extern float	MaterialSpecPower;						//	材质镜面高光强度
uniform extern float3	MaterialEmissive;						//	材质自发光颜色

//////////////////////////////////////////////
bool 	DiffuseEnabled = false;
bool	DetailEnabled = false;
float2	DiffuseUVTile;
float2 	DetailUVTile;

texture	DiffuseTexture 
<
	string Label = "颜色贴图"; 
>;

sampler	SamplerDiffuse = sampler_state
{
	Texture	  =	(DiffuseTexture);
	MipFilter =	LINEAR;
	MinFilter =	LINEAR;
	MagFilter =	LINEAR;
	MipMapLodBias = -2.5f;
};

texture	DetailTexture 
<
	string SasUiLabel = "细节贴图"; 
>;

sampler	SamplerDetail = sampler_state
{
	Texture	  =	(DetailTexture);
	MipFilter =	LINEAR;
	MinFilter =	LINEAR;
	MagFilter =	LINEAR;
	MipMapLodBias = -2.5f;
};