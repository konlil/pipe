#define MAX_TOTAL_LIGHTS	4

#define LIGHT_TYPE_DIR		0
#define LIGHT_TYPE_POINT	1
#define LIGHT_TYPE_SPOT		2

#define NO_COLOR	float4(0, 0, 0, 0);

//shared variables
shared uniform extern int		TotalLights;

//light structure
struct Light
{
	float enabled;
	float type;
	float3 color;
	float3 position;
	float3 direction;
	float4 spot_data;  //四分量分别为: range, falloff, theta, phi
};

//light array
shared Light Lights[MAX_TOTAL_LIGHTS];


//light functions
//-----------------------------------------------------------------------------------------------
//	函数:	Attenuation
//	
//	作者:	E. Riba (Riki) 12.08.2009
//
//	描述:	计算随距离增大而减小的衰减值，例如lRange为10，falloff为1，则距离光源为5的顶点的光照只有一半
//				
//-----------------------------------------------------------------------------------------------
float Attenuation(float lDistance, float lRange, float falloff)
{
	float att = pow(saturate((lRange - lDistance) / lRange), falloff);	
    // 以下为简化计算方程
    //float att = 1 - saturate(lDistance*falloff / lRange);
    return att;    
}

//-----------------------------------------------------------------------------------------------
//	函数:	Lambertian因子
//	
//	作者:	E. Riba (Riki) 07.08.2009
//
//	描述:	基本的lambertian光照方程
//				
//-----------------------------------------------------------------------------------------------
float3 LambertianComponent(in float3 directionToLight, in float3 worldNormal, in float3 lightColor)
{
	float3 NdotL = saturate(dot(directionToLight, worldNormal));
	return NdotL * lightColor;
}

//-----------------------------------------------------------------------------------------------
//	函数:	Specular分量
//	
//	作者:	E. Riba (Riki) 08.08.2009
//
//	描述:	基于Phong公式的镜面高光公式
//				
//-----------------------------------------------------------------------------------------------
float3 SpecularComponent(in float3 directionToLight, in float3 worldNormal, in float3 worldPosition, in float3 lightColor, in float3 specularColor, in float specularPower)
{
	float3 specular = 0;
	if(length(specularColor)>0)
	{
		// 计算反射光方向
		float3 reflectionVector = normalize(reflect(-directionToLight, worldNormal));
		
		// 计算像素指向相机的向量，即视线反方向
		float3 directionToCamera = normalize(CameraPos - worldPosition);
		
		// 计算视线反方向和反射光方向的夹角
		float  VdotR = saturate(dot(directionToCamera,reflectionVector));
		
		// 计算镜面高光颜色
		specular = lightColor * specularColor *pow(VdotR,specularPower);
		specular = saturate(specular);
	}	
	return specular;
}


//-----------------------------------------------------------------------------------------------
//	函数:	BlinnPhongSpecular
//	
//	作者:	E. Riba (Riki) 12.08.2009
//
//	描述:	基于Bilnn Phong公式的镜面高光公式
//				
//-----------------------------------------------------------------------------------------------
float3 BlinnPhongSpecular( in float3 directionToLight, in float3 worldNormal, in float3 worldPosition, in float3 lightColor, in float3 specularColor, in float3 specularPower)
{    
	float3 specular = 0;
	if(length(specularColor)>0)
	{
		//计算像素指向相机的向量，即视线反方向
		float3 viewer = normalize(CameraPos - worldPosition);
		
		// 计算入射光反方向和视线反方向之间的角平分线方向
		float3 half_vector = normalize(directionToLight + viewer);
	 
		// 计算角平分线与法线方向的夹角
		float  NdotH = saturate(dot( worldNormal, half_vector));
	 
		// 计算镜面高光颜色
		specular = lightColor * specularColor * pow( NdotH, specularPower) ;
		specular = saturate(specular);
    }   
    return specular;
}

float3 CalcSingleLight(Light light, float3 world_position, float3 world_normal, float3 diffuse, float3 specular, float spec_power)
{
	float3 color = light.color;
	float type = light.type;
	float attenuation = 0;	//衰减值
	float3 dir_to_src;  //顶点指向光源，入射光反方向
	
	if(type == LIGHT_TYPE_DIR)  //方向光不衰减
	{
		dir_to_src = -normalize(light.direction);
		attenuation = 1;
	}
	else
	{
		float range = light.spot_data.x;
		float falloff = light.spot_data.y;
		float cos_theta = cos(light.spot_data.z);
		float cos_phi = cos(light.spot_data.w);
		
		//顶点到光源的距离
		float dist = distance(light.position, world_position);			
	
		dir_to_src = normalize(light.position - world_position);
		
		//点光，且顶点位于点光源范围内
		if( (type == LIGHT_TYPE_POINT) && (range > dist) )
		{
			attenuation = Attenuation(dist, range, falloff);
		}
		//聚光，且顶点位于光源范围内
		else if( type == LIGHT_TYPE_SPOT )
		{
			if( range > dist )
			{
				//距离衰减值
				float dist_atten = Attenuation(dist, range, falloff);  //距离衰减
		
				//计算当前光源指向顶点方向和聚光灯光线方向的夹角Alpha的余弦值
				float cos_alpha = saturate(dot(-dir_to_src, normalize(light.direction))	);
				
				//聚光灯内外锥的衰减
				float spot_atten = 0.0f;
				if( cos_alpha > cos_theta )
					spot_atten = 1.0f;
				else if ( cos_alpha > cos_phi )	//指数衰减
					spot_atten = pow( (cos_alpha - cos_phi)/(cos_theta - cos_phi), falloff);
				
				attenuation = dist_atten * spot_atten;
			}
			else
			{
				attenuation = 0;
			}
		}
	}
	
	//漫反射分量
	float3 lc = LambertianComponent(dir_to_src, world_normal, color);
	
	//镜面反射（phong模型）
	float3 sc = SpecularComponent(dir_to_src, world_normal, world_position, color, specular, spec_power);
	//(Blinn Phong模型）
	//float3 sc = BlinnPhongSpecular(dir_to_src, world_normal, world_position, color, specular, spec_power);

	return (lc + sc) * (attenuation * diffuse.rgb);	
}


