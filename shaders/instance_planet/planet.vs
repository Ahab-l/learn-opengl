#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in mat4 aInstanceMatrix;

out vec2 TexCoords;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{   
    mat4 jjyy=aInstanceMatrix;
    if(gl_InstanceID==0){
        jjyy=mat4(4.0, 0.0, 0.0, 0.0,
	   	0.0, 4.0, 0.0, 0.0,
		0.0, 0.0, 4.0, 0.0,
		0, 0, 0, 1.0);
    }
    TexCoords = aTexCoords;
    gl_Position = projection * view * jjyy * vec4(aPos, 1.0f); 
}