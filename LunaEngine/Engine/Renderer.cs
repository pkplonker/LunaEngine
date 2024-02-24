﻿using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public class Renderer
{
	public GL Gl { get; private set; }



	private Framebuffer framebuffer;
	public Silk.NET.OpenGL.Texture renderTexture { get; private set; }

	private const int colorVal = 50;
	private Vector4D<int> clearColor = new(colorVal, colorVal, colorVal, 255);
	private Shader shader;
	private Texture texture;
	private IWindow window;
	private Vector2D<float> imageSize;
	private Vector2D<int> windowSize;
	public List<IRenderable> renderables = new List<IRenderable>();

	public void RenderUpdate(double deltaTime, Matrix4x4? view, Matrix4x4? projection)
	{
		using (var tracker = new PerformanceTracker(nameof(RenderUpdate)))
		{
			unsafe
			{
				Gl.Viewport(0, 0, (uint) imageSize.X, (uint) imageSize.Y);
				Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

				Gl.Enable(EnableCap.DepthTest);
				Gl.ClearColor(clearColor);
				Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
				
				shader.Use();
				texture.Bind(TextureUnit.Texture0);
				shader.SetUniform("Texture0", 0);
				shader.SetUniform("UseTexture", false);
				foreach (var renderable in renderables)
				{
					renderable.Bind(Gl);
				}
				

				Gl.Viewport(0, 0, (uint) windowSize.X, (uint) windowSize.Y);

				Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			}

			var difference = (float) (window.Time * 100);

			var model = Matrix4x4.CreateRotationY(MathExtensions.DegreesToRadians(difference)) *
			            Matrix4x4.CreateRotationX(MathExtensions.DegreesToRadians(difference));
			shader.SetUniform("uModel", model);

			if (view.HasValue)
				shader.SetUniform("uView", view.Value);
			if (projection.HasValue)
				shader.SetUniform("uProjection", projection.Value);
		}
	}

	public void Resize(Vector2D<int> size) => windowSize = size;

	public void Load(IWindow window)
	{
		using var tracker = new PerformanceTracker(nameof(RenderUpdate));
		unsafe
		{
			Gl = GL.GetApi(window);
			windowSize = window.Size;
			imageSize = (Vector2D<float>) window.Size;

			this.window = window;

			shader = new Shader(Gl, @"/resources/shaders/unlitvertex.glsl",
				@"/resources/shaders/unlitfragment.glsl");
			texture = new Texture(Gl, @"/resources/textures/test.png");
			Gl.GenFramebuffers(1, out framebuffer);
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

			Gl.GenTextures(1, out Silk.NET.OpenGL.Texture rt);
			renderTexture = rt;
			Gl.BindTexture(TextureTarget.Texture2D, renderTexture.Handle);
			Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, 3860, 2140, 0, PixelFormat.Rgba,
				PixelType.UnsignedByte, null);

			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int) TextureMinFilter.Linear);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int) TextureMagFilter.Linear);

			Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
				TextureTarget.Texture2D, renderTexture.Handle, 0);
		}
	}

	public void Close()
	{
		shader.Dispose();
		texture.Dispose();
	}

	public void SetRenderTargetSize(Vector2D<float> size)
	{
		imageSize = size;
		unsafe
		{
			if (framebuffer.Handle != 0)
			{
				Gl.DeleteFramebuffer(framebuffer.Handle);
				Gl.DeleteTexture(renderTexture.Handle);
			}

			Gl.GenFramebuffers(1, out framebuffer);
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

			Gl.GenTextures(1, out Silk.NET.OpenGL.Texture rt);
			renderTexture = rt;
			Gl.BindTexture(TextureTarget.Texture2D, renderTexture.Handle);
			Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint) size.X, (uint) size.Y, 0,
				PixelFormat.Rgba,
				PixelType.UnsignedByte, null);

			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int) TextureMinFilter.Linear);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int) TextureMagFilter.Linear);

			Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
				TextureTarget.Texture2D, renderTexture.Handle, 0);
		}
	}
}