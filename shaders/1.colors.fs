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
in vec3 normal;
in vec3 fragpos;
out vec4 fragcolor;
  
uniform vec3 objectcolor;
uniform vec3 lightcolor;
uniform vec3 lightpos;
uniform vec3 camerpos;
void main()
{
    float ambientstrenth =0.1;
    float specularstrength=0.0;



    vec3 ambient =ambientstrenth*lightcolor;
    vec3 norm = normalize(normal);
    vec3 lightdir =normalize(lightpos-fragpos);
    float diff =max(dot(norm,lightdir),0.0);
    vec3 diffuse = diff* lightcolor;

    vec3 viewdir =normalize(camerpos-fragpos);
    vec3 reflectdir = reflect(-lightdir,norm);

    float spec = pow(max(dot(viewdir,reflectdir),0.0f),32);
    vec3  specular =specularstrength*spec *lightcolor;

    vec3 result = (ambient+diffuse+spec) * objectcolor;
    fragcolor = vec4(result, 1.0);
}


