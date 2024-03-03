using System.Drawing;
using System.Numerics;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public class Renderer
{
	public GL Gl { get; private set; }

	private const int colorVal = 50;
	private Vector4D<int> clearColor = new(colorVal, colorVal, colorVal, 255);
	private Texture texture;
	public Vector2D<int> WindowSize;
	private Shader lastShader;
	public int DrawCalls { get; private set; }
	public int MaterialsUsed { get; private set; }
	public int ShadersUsed { get; private set; }
	public uint Triangles { get; set; }
	public uint Vertices { get; set; }
	private Dictionary<Scene, IRenderTarget> sceneRenderTargets = new Dictionary<Scene, IRenderTarget>();

	public void AddScene(Scene scene, Vector2D<uint> size, out IRenderTarget renderTarget, bool toFrameBuffer)
	{
		if (!sceneRenderTargets.ContainsKey(scene))
		{
			renderTarget = GenerateIRenderTarget(size.X, size.Y,toFrameBuffer);
			sceneRenderTargets.Add(scene, renderTarget);
		}

		renderTarget = sceneRenderTargets[scene];
	}

	public void RenderUpdate()
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
				foreach (var kvp in sceneRenderTargets)
				{
					DrawScene(kvp.Value, kvp.Key);
				}

				Gl.Viewport(0, 0, (uint) WindowSize.X, (uint) WindowSize.Y);
				Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			}
		}
	}

	private void DrawScene(IRenderTarget renderTarget, Scene scene)
	{
		renderTarget.Bind(Gl);
		Gl.Enable(EnableCap.DepthTest);
		Gl.ClearColor(clearColor);
		Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
		if (scene.ActiveCamera == null)
		{
			Debug.Warning("No active camera to render with");
			return;
		}

		var renderPassData = new RenderPassData(scene.ActiveCamera.GetView(), scene.ActiveCamera.GetProjection());
		foreach (var component in scene.GameObjects.Select(go => go?.GetComponent<IRenderableComponent>()))
		{
			component?.Render(this, renderPassData);
		}
	}

	public void Resize(Vector2D<int> size)
	{
		WindowSize = size;
		foreach (var target in sceneRenderTargets)
		{
			target.Value?.ResizeWindow(Gl,(uint)size.X, (uint)size.Y);
		}
	}

	public void Load(IWindow window)
	{
		using var tracker = new PerformanceTracker(nameof(RenderUpdate));
		unsafe
		{
			Gl = GL.GetApi(window);
			WindowSize = window.Size;

			Gl.Enable(GLEnum.CullFace);
			Gl.CullFace(GLEnum.Back);
			Gl.FrontFace(FrontFaceDirection.Ccw);
		}
	}

	private unsafe IRenderTarget GenerateIRenderTarget(uint sizeX, uint sizeY, bool useFrameBuffer)
	{
		if (useFrameBuffer)
		{
			return GenerateFrameBufferRenderTarget(sizeX, sizeY);
		}
		else
		{
			return GenerateRenderTarget();
		}
	}

	private IRenderTarget GenerateRenderTarget()
	{
		return new RenderTarget(WindowSize.X,WindowSize.Y);
	}

	private unsafe FrameBufferRenderTarget GenerateFrameBufferRenderTarget(uint sizeX, uint sizeY)
	{
		Gl.GenFramebuffers(1, out Framebuffer framebuffer);
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

		Gl.GenTextures(1, out Silk.NET.OpenGL.Texture rt);
		Gl.BindTexture(TextureTarget.Texture2D, rt.Handle);
		Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, sizeX, sizeY, 0, PixelFormat.Rgba,
			PixelType.UnsignedByte, null);

		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
			(int) TextureMinFilter.Linear);
		Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
			(int) TextureMagFilter.Linear);

		Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
			TextureTarget.Texture2D, rt.Handle, 0);
		return new FrameBufferRenderTarget(framebuffer, rt, new Vector2D<int>((int) sizeX, (int) sizeY));
	}

	public void SetRenderTargetSize(Scene scene, Vector2D<float> size)
	{
		unsafe
		{
			if (sceneRenderTargets.TryGetValue(scene, out var rt))
			{
				rt.ResizeViewport(Gl, (uint) size.X, (uint) size.Y);
			}
		}
	}

	public void SetRenderTargetSize(RenderTarget target, Vector2D<float> size)
	{
		unsafe
		{
			target.ResizeViewport(Gl, (uint) size.X, (uint) size.Y);
		}
	}

	public void Close()
	{
		texture?.Dispose();
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
		UseShader(shader);
		MaterialsUsed++;
		material.Use(this, data, modelMatrix);
	}

	public IRenderTarget? GetSceneRenderTarget(Scene scene) =>
		sceneRenderTargets.TryGetValue(scene, out var rt) ? rt : null;
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