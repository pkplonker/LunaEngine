using System.Drawing;
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
	private Texture texture;
	private IWindow window;
	public Vector2D<float> ViewportSize;
	public Vector2D<int> WindowSize;
	private Shader lastShader;
	public int DrawCalls { get; private set; }
	public int MaterialsUsed { get; private set; }
	public int ShadersUsed { get; private set; }
	public uint Triangles { get; set; }
	public uint Vertices { get; set; }

	public void RenderUpdate(Matrix4x4? view, Matrix4x4? projection)
	{
		using (var tracker = new PerformanceTracker(nameof(RenderUpdate)))
		{
			DrawCalls = 0;
			ShadersUsed = 0;
			lastShader = null;
			MaterialsUsed = 0;
			Triangles = 0;
			Vertices = 0;
			unsafe
			{
				Gl.Viewport(0, 0, (uint) ViewportSize.X, (uint) ViewportSize.Y);
				Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

				Gl.Enable(EnableCap.DepthTest);
				Gl.ClearColor(clearColor);
				Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
				var renderPassData = new RenderPassData(view.Value, projection.Value);
				foreach (var go in SceneController.ActiveScene.GameObjects)
				{
					var component = go?.GetComponent<IRenderableComponent>();
					component?.Render(this, renderPassData);
				}

				Gl.Viewport(0, 0, (uint) WindowSize.X, (uint) WindowSize.Y);

				Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			}
		}
	}

	public void Resize(Vector2D<int> size) => WindowSize = size;

	public void Load(IWindow window)
	{
		using var tracker = new PerformanceTracker(nameof(RenderUpdate));
		unsafe
		{
			Gl = GL.GetApi(window);
			WindowSize = window.Size;
			ViewportSize = (Vector2D<float>) window.Size;

			this.window = window;

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
		texture?.Dispose();
	}

	public void SetRenderTargetSize(Vector2D<float> size)
	{
		ViewportSize = size;
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

	public unsafe void DrawElements(PrimitiveType primativeType, uint indicesLength, DrawElementsType elementsTyp)
	{
		DrawCalls++;
		Triangles += indicesLength / 3;
		Vertices += indicesLength;

		Gl.DrawElements(primativeType, indicesLength, elementsTyp, null);
	}

	public void UseShader(Shader? shader)
	{
		if (shader == null) return;

		if (lastShader != shader)
		{
			ShadersUsed++;
			shader?.Use();
			lastShader = shader;
		}
	}

	public void UseMaterial(Material material, RenderPassData data, Matrix4x4 modelMatrix)
	{
		if (material == null || material.Shader == null) return;
		var shader = material.Shader;
		MaterialsUsed++;
		material.Use(this, data, modelMatrix);
	}
}

public struct RenderPassData
{
	public readonly Matrix4x4 View;
	public readonly Matrix4x4 Projection;

	public RenderPassData(Matrix4x4 view, Matrix4x4 projection)
	{
		View = view;
		Projection = projection;
	}
}