//#version 330 core
//in vec3 Normal;
//in vec3 FragPos;
//out vec4 FragColor;
  
//uniform vec3 objectColor;
//uniform vec3 lightColor;
//uniform vec3 lightPos;
//uniform vec3 camerPos;
//void main()
//{
//    float ambientStrenth =0.1;
//    float specularStrength=0.0;



//    vec3 ambient =ambientStrenth*lightColor;
//    vec3 norm = normalize(Normal);
//    vec3 lightDir =normalize(lightPos-FragPos);
//    float diff =max(dot(norm,lightDir),0.0);
//    vec3 diffuse = diff* lightColor;

//    vec3 viewdir =normalize(camerPos-FragPos);
//    vec3 reflectdir = reflect(-lightDir,norm);

//    float spec = pow(max(dot(viewdir,reflectdir),0.0f),32);
//    vec3  specular =specularStrength*spec *lightColor;

//    vec3 result = (ambient+diffuse+spec) * objectColor;
//    FragColor = vec4(result, 1.0);
//}
#version 330 core

struct Material {

    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;
    float shininess;
};
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
struct SpotLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 direction;
    float cutoff;
    float outcutoff;
};
#define NR_POINT_LIGHTS 4
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform DirLight dirLight;
uniform SpotLight spotLight;
uniform Material material;  
uniform vec3 camerapos; 
in vec3 normal;
in vec3 fragpos;
in vec2 TexCoords;

out vec4 FragColor;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // 衰减
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}
vec3 CalcSpotLight(SpotLight spotLight,vec3 norm,vec3 FragPos,vec3 viewDir)
{
    vec3 lightdir =normalize(spotLight.position-fragpos);    

    float theta = dot(lightdir, normalize(-spotLight.direction));
    float epsilon   = spotLight.cutoff - spotLight.outcutoff;
    float intensity = clamp((theta - spotLight.outcutoff) / epsilon, 0.0, 1.0);  
;

    vec3 ambient =spotLight.ambient*vec3(texture(material.diffuse,TexCoords));

    float diff =max(dot(norm,lightdir),0.0);
    vec3 diffuse = diff*vec3(texture(material.diffuse,TexCoords))*spotLight.diffuse;

    vec3 reflectdir = reflect(-lightdir,norm);
    float spec = pow(max(dot(viewDir,reflectdir),0.0f),material.shininess);
    vec3 specular =spotLight.specular*spec*(vec3(1.0f));
    
    float distance =length(spotLight.position-fragpos);
    float attenuation =1.0/(spotLight.constant+spotLight.linear*distance+spotLight.quadratic*(distance*distance));

    vec3 result=ambient*attenuation+intensity*diffuse*attenuation+specular*attenuation*intensity;
    //result=ambient+intensity*4*pow(theta,384)*diffuse*attenuation+pow(theta,128)*specular*attenuation*intensity;
    //if(vec3(texture(material.emission,TexCoord)).y>10.0f/256){
    //    result=vec3(texture(material.emission,TexCoord));
    //}
    return result;

}
void main()
{
    // 属性
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(camerapos - fragpos);

    // 第一阶段：定向光照
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    // 第二阶段：点光源 
    for(int i = 0; i < 4; i++)
        result += CalcPointLight(pointLights[i], norm, fragpos, viewDir);    
    // 第三阶段：聚光
    //result += CalcSpotLight(spotLight, norm, fragpos, viewDir);    

    FragColor = vec4(result, 1.0);
}


