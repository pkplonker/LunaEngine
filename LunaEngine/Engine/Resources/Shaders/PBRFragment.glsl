#version 330 core

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;
uniform sampler2D Texture4;

uniform int UseTexture;
uniform int uAlbedo;
uniform int uNormal;
uniform int uMetallic;
uniform int uRoughness;
uniform int uAO;

in vec2 oUV;

out vec4 FragColor;

void main()
{
	
	float val=0;
	if(uAlbedo +uNormal+uMetallic+uRoughness+uAO > 10)
	{
		val=1;
	}
	val = 88.0f/255.0f;
	//FragColor = vec4(val,val,val, 1.0f);
	//FragColor = vec4(1.0,0.0,1.0, 1.0f);


	FragColor = texture(Texture0, oUV);
	
}