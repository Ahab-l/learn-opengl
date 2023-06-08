#version 330 core
in float spec;
in float diff;


out vec4 fragcolor;
  
uniform vec3 objectcolor;
uniform vec3 lightcolor;


void main()
{
    float ambientstrenth =0.1;
    vec3 ambient =ambientstrenth*lightcolor;


    vec3 diffuse = diff* lightcolor;
    
    float specularstrength=0.5;

    vec3 specular =specularstrength*spec *lightcolor;

    vec3 result = (ambient+diffuse) * objectcolor;
    fragcolor = vec4(result, 1.0);
}


