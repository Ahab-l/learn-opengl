#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;



out float spec;
out float diff;

uniform vec3 alight;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec3 fragpos = vec3(view*model * vec4(aPos, 1.0));
    vec3 lightPos = vec3(view*vec4(alight,1.0));
    vec3 normal =  mat3(transpose(inverse(view * model))) * aNormal;
    
    vec3 lightdir=normalize((lightPos-fragpos));
    vec3 reflectdir=reflect(-lightdir,normal);
    vec3 viewdir=normalize(-fragpos);
    
    diff =pow(max(dot(normal,lightdir),0.0),4);
    


    spec = pow(max(dot(viewdir,reflectdir),0.0f),32);


    gl_Position = projection * view * model*vec4(aPos, 1.0);
}