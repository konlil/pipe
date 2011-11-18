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
	float4 spot_data;  //�ķ����ֱ�Ϊ: range, falloff, theta, phi
};

//light array
shared Light Lights[MAX_TOTAL_LIGHTS];


//light functions
//-----------------------------------------------------------------------------------------------
//	����:	Attenuation
//	
//	����:	E. Riba (Riki) 12.08.2009
//
//	����:	����������������С��˥��ֵ������lRangeΪ10��falloffΪ1��������ԴΪ5�Ķ���Ĺ���ֻ��һ��
//				
//-----------------------------------------------------------------------------------------------
float Attenuation(float lDistance, float lRange, float falloff)
{
	float att = pow(saturate((lRange - lDistance) / lRange), falloff);	
    // ����Ϊ�򻯼��㷽��
    //float att = 1 - saturate(lDistance*falloff / lRange);
    return att;    
}

//-----------------------------------------------------------------------------------------------
//	����:	Lambertian����
//	
//	����:	E. Riba (Riki) 07.08.2009
//
//	����:	������lambertian���շ���
//				
//-----------------------------------------------------------------------------------------------
float3 LambertianComponent(in float3 directionToLight, in float3 worldNormal, in float3 lightColor)
{
	float3 NdotL = saturate(dot(directionToLight, worldNormal));
	return NdotL * lightColor;
}

//-----------------------------------------------------------------------------------------------
//	����:	Specular����
//	
//	����:	E. Riba (Riki) 08.08.2009
//
//	����:	����Phong��ʽ�ľ���߹⹫ʽ
//				
//-----------------------------------------------------------------------------------------------
float3 SpecularComponent(in float3 directionToLight, in float3 worldNormal, in float3 worldPosition, in float3 lightColor, in float3 specularColor, in float specularPower)
{
	float3 specular = 0;
	if(length(specularColor)>0)
	{
		// ���㷴��ⷽ��
		float3 reflectionVector = normalize(reflect(-directionToLight, worldNormal));
		
		// ��������ָ������������������߷�����
		float3 directionToCamera = normalize(CameraPos - worldPosition);
		
		// �������߷�����ͷ���ⷽ��ļн�
		float  VdotR = saturate(dot(directionToCamera,reflectionVector));
		
		// ���㾵��߹���ɫ
		specular = lightColor * specularColor *pow(VdotR,specularPower);
		specular = saturate(specular);
	}	
	return specular;
}


//-----------------------------------------------------------------------------------------------
//	����:	BlinnPhongSpecular
//	
//	����:	E. Riba (Riki) 12.08.2009
//
//	����:	����Bilnn Phong��ʽ�ľ���߹⹫ʽ
//				
//-----------------------------------------------------------------------------------------------
float3 BlinnPhongSpecular( in float3 directionToLight, in float3 worldNormal, in float3 worldPosition, in float3 lightColor, in float3 specularColor, in float3 specularPower)
{    
	float3 specular = 0;
	if(length(specularColor)>0)
	{
		//��������ָ������������������߷�����
		float3 viewer = normalize(CameraPos - worldPosition);
		
		// ��������ⷴ��������߷�����֮��Ľ�ƽ���߷���
		float3 half_vector = normalize(directionToLight + viewer);
	 
		// �����ƽ�����뷨�߷���ļн�
		float  NdotH = saturate(dot( worldNormal, half_vector));
	 
		// ���㾵��߹���ɫ
		specular = lightColor * specularColor * pow( NdotH, specularPower) ;
		specular = saturate(specular);
    }   
    return specular;
}

float3 CalcSingleLight(Light light, float3 world_position, float3 world_normal, float3 diffuse, float3 specular, float spec_power)
{
	float3 color = light.color;
	float type = light.type;
	float attenuation = 0;	//˥��ֵ
	float3 dir_to_src;  //����ָ���Դ������ⷴ����
	
	if(type == LIGHT_TYPE_DIR)  //����ⲻ˥��
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
		
		//���㵽��Դ�ľ���
		float dist = distance(light.position, world_position);			
	
		dir_to_src = normalize(light.position - world_position);
		
		//��⣬�Ҷ���λ�ڵ��Դ��Χ��
		if( (type == LIGHT_TYPE_POINT) && (range > dist) )
		{
			attenuation = Attenuation(dist, range, falloff);
		}
		//�۹⣬�Ҷ���λ�ڹ�Դ��Χ��
		else if( (type == LIGHT_TYPE_SPOT) && (range > dist) )
		{
			//����˥��ֵ
			float dist_atten = Attenuation(dist, range, falloff);  //����˥��
		
			//���㵱ǰ��Դָ�򶥵㷽��;۹�ƹ��߷���ļн�Alpha������ֵ
			float cos_alpha = saturate(dot(-dir_to_src, normalize(light.direction))	);
			
			//�۹������׶��˥��
			float spot_atten = 0.0f;
			if( cos_alpha > cos_theta )
				spot_atten = 1.0f;
			else if ( cos_alpha > cos_phi )	//ָ��˥��
				spot_atten = pow( (cos_alpha - cos_phi)/(cos_theta - cos_phi), falloff);
			
			attenuation = dist_atten * spot_atten;
		}
	}
	
	//���������
	float3 lc = LambertianComponent(dir_to_src, world_normal, color);
	
	//���淴�䣨phongģ�ͣ�
	float3 sc = SpecularComponent(dir_to_src, world_normal, world_position, color, specular, spec_power);
	//(Blinn Phongģ�ͣ�
	//float3 sc = BlinnPhongSpecular(dir_to_src, world_normal, world_position, color, specular, spec_power);

	return (lc + sc) * (attenuation * diffuse.rgb);	
}

