
shared uniform extern float3 AmbientColor = float3(0.1, 0.1, 0.1);

uniform extern float4x4 World;
uniform extern float4x4 View;
uniform extern float4x4 Projection;

//----------------------------------------------------
//	���ʲ���
//----------------------------------------------------
uniform extern float3	MaterialDiffuse;						//	������������ɫ
uniform extern float3	MaterialSpecular;						//	���ʾ���߹���ɫ
uniform extern float	MaterialSpecPower;						//	���ʾ���߹�ǿ��
uniform extern float3	MaterialEmissive;						//	�����Է�����ɫ

//////////////////////////////////////////////
bool 	DiffuseEnabled = false;
bool	DetailEnabled = false;
float2	DiffuseUVTile;
float2 	DetailUVTile;

texture	DiffuseTexture 
<
	string Label = "��ɫ��ͼ"; 
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
	string SasUiLabel = "ϸ����ͼ"; 
>;

sampler	SamplerDetail = sampler_state
{
	Texture	  =	(DetailTexture);
	MipFilter =	LINEAR;
	MinFilter =	LINEAR;
	MagFilter =	LINEAR;
	MipMapLodBias = -2.5f;
};