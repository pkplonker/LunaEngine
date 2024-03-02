using System.Drawing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Engine;

public class RenderTarget
{
	public Silk.NET.OpenGL.Texture texture { get; private set; }
	public Framebuffer frameBuffer { get; private set; }
	public Vector2D<int> ViewportSize { get; set; }
	public IntPtr GetHandlePtr() => (IntPtr) texture.Handle;

	public RenderTarget(Framebuffer framebuffer, Silk.NET.OpenGL.Texture texture, Vector2D<int> size)
	{
		this.frameBuffer = framebuffer;
		this.texture = texture;
		this.ViewportSize = size;
	}

	public void ResizeTexture(GL gl, uint sizeX, uint sizeY)
	{
		unsafe
		{
			ViewportSize = new Vector2D<int>((int) sizeX, (int) sizeY);
			gl.BindTexture(TextureTarget.Texture2D, texture.Handle);
			gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, sizeX, sizeY, 0, PixelFormat.Rgba,
				PixelType.UnsignedByte, null);
		}
	}
}