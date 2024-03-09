using Silk.NET.OpenGL;

namespace Engine;

public interface ITexture
{
	uint Width { get; }
	uint Height { get; }
	string Path { get; set; }
	Guid GUID { get; set; }
	void Bind(TextureUnit textureSlot = TextureUnit.Texture0);
	void Dispose();
}