using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using StbImageSharp;
using File = System.IO.File;

namespace Engine;

[Serializable]
public class Texture : IDisposable
{
	private uint handle;
	private GL gl;

	public string Path { get; set; }
	public TextureType Type { get; }

	public unsafe Texture(GL gl, string path, TextureType type = TextureType.None)
	{
		this.gl = gl;
		Path = path;
		Type = type;
		handle = this.gl.GenTexture();
		Bind();

		var img = ImageResult.FromMemory(File.ReadAllBytes(path.MakeAbsolute()), ColorComponents.RedGreenBlueAlpha);
		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint) img.Width, (uint) img.Height, 0,
			PixelFormat.Rgba, PixelType.UnsignedByte, null);

		SetParameters();
	}

	private void SetParameters()
	{
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
			(int) GLEnum.LinearMipmapLinear);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
		gl.GenerateMipmap(TextureTarget.Texture2D);
	}

	public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
	{
		gl.ActiveTexture(textureSlot);
		gl.BindTexture(TextureTarget.Texture2D, handle);
	}

	public void Dispose()
	{
		gl.DeleteTexture(handle);
	}
}