#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec3 vTangent;
layout (location = 3) in vec2 vUv;
layout (location = 4) in vec3 vBiTangent;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;


out vec2 oUV;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
	oUV = vUv;
}