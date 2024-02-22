#version 330 core

uniform sampler2D Texture0;
uniform int UseTexture;

in vec2 oUV;

out vec4 FragColor;

void main()
{
	if(UseTexture != 1)
	{
		FragColor = vec4(1.0f, 0.0f, 1.0f, 1.0f);
	}
	else
	{
	    FragColor = texture(Texture0, oUV);
	}
}