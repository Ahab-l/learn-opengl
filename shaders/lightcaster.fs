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
struct Light {
    vec3 position;
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float cutoff;
    float outcutoff;

    float constant;
    float linear;
    float quadratic;

};



in vec3 normal;
in vec3 fragpos;
in vec2 TexCoord;

out vec4 fragcolor;



uniform Material material;  
uniform Light light;
uniform vec3 camerapos;

void main()
{
    vec3 lightdir =normalize(camerapos-fragpos);    

    float theta = dot(lightdir, normalize(-light.direction));

    float epsilon   = light.cutoff - light.outcutoff;
    float intensity = clamp((theta - light.outcutoff) / epsilon, 0.0, 1.0);  

    vec3 result=vec3(0.0f);

    vec3 ambient =light.ambient*vec3(texture(material.diffuse,TexCoord));

    vec3 norm = normalize(normal);

    float diff =max(dot(norm,lightdir),0.0);
    vec3 diffuse = diff*vec3(texture(material.diffuse,TexCoord))*light.diffuse;

    vec3 viewdir =normalize(camerapos-fragpos);
    vec3 reflectdir = reflect(-lightdir,norm);

    float spec = pow(max(dot(viewdir,reflectdir),0.0f),material.shininess);
    vec3 specular =light.specular*spec*(vec3(1.0f));
    
    float distance =length(light.position-fragpos);
    float attenuation =1.0/(light.constant+light.linear*distance+light.quadratic*(distance*distance));

    result=ambient+intensity*diffuse*attenuation+specular*attenuation*intensity;
    //result=ambient+intensity*4*pow(theta,384)*diffuse*attenuation+pow(theta,128)*specular*attenuation*intensity;
    //if(vec3(texture(material.emission,TexCoord)).y>10.0f/256){
    //    result=vec3(texture(material.emission,TexCoord));
    //}
    fragcolor = vec4(result,1.0f);

}


