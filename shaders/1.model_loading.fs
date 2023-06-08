#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
in vec3 fragpos;
in vec3 normal;

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
#define NR_POINT_LIGHTS 4
uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_reflect1;
uniform samplerCube skybox;
uniform float shininess;
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform DirLight dirLight;
uniform vec3 camerapos;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(texture_diffuse1, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(texture_diffuse1, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(texture_specular1, TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    // 衰减
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(texture_diffuse1, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(texture_diffuse1, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(texture_specular1, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

void main()
{   vec3 norm = normalize(normal);
    vec3 viewDir = normalize(camerapos - fragpos);
    vec3 reflect = normalize(reflect(-viewDir,norm));

    vec3 result =  CalcDirLight(dirLight, norm, viewDir);

    for(int i=0;i<NR_POINT_LIGHTS;i++){
        result += CalcPointLight(pointLights[i], norm, fragpos, viewDir);
    } 
    vec3 reflectWeight =texture(texture_reflect1,TexCoords).rgb;
    vec3 skyLight=texture(skybox,reflect).rgb*reflectWeight+result;
    //FragColor = vec4(texture(texture_reflect1, TexCoords).rgb, 1.0);
    FragColor=vec4(skyLight,1.0f);
    float ratio = 1.00 / 1.52;
    vec3 I = normalize(fragpos - camerapos);
    vec3 R = refract(I, normalize(norm), ratio);
    //FragColor = vec4(texture(skybox, R).rgb, 1.0);
}

