#version 330 core

uniform sampler2D Texture0;
uniform int UseTexture;

in vec2 oUV;

out vec4 FragColor;

void main()
{
	if(UseTexture == 0)
	{
		float val = 88.0f/255.0f;
		FragColor = vec4(val,val,val, 1.0f);
	}
	else
	{
	    FragColor = texture(Texture0, oUV);
	}
}