//shared variables, cached in effect pool
shared uniform extern float4 FogColor = {0.1, 0.2, 0.3, 1.0};
shared uniform extern float FogStart = 50;
shared uniform extern float FogEnd = 400;


//global variables
uniform extern float3 CameraPos;
uniform extern bool FogEnabled;


//fog functions
float3 LinearFog(float3 color, float3 world_pos)
{			
	float d = length(world_pos - CameraPos);  
	float l = saturate((d - FogStart) / (FogEnd - FogStart));
	color.rgb = lerp(color, FogColor.rgb, l);  
	return color;	
}
