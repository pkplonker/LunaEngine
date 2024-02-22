#version 330 core

layout (location = 0) in vec4 vPos;
layout (location = 1) in vec2 vUv;

out vec2 oUV;

void main()
{
	gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
	oUV = vUv;
}