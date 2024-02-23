using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public class Renderer
{
	public GL Gl { get; private set; }

	private BufferObject<float> vbo;
	private BufferObject<uint> ebo;
	private VertexArrayObject<float, uint> vao;

	private readonly float[] VERTICES =
	{
		//X    Y      Z     U   V
		-0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
		0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
		0.5f, 0.5f, -0.5f, 1.0f, 0.0f,
		-0.5f, 0.5f, -0.5f, 0.0f, 0.0f,

		-0.5f, -0.5f, 0.5f, 0.0f, 1.0f,
		0.5f, -0.5f, 0.5f, 1.0f, 1.0f,
		0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
		-0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
	};

	private readonly uint[] INDICES =
	{
		0, 1, 2,
		2, 3, 0,

		4, 5, 6,
		6, 7, 4,

		0, 3, 7,
		7, 4, 0,

		1, 2, 6,
		6, 5, 1,

		0, 1, 5,
		5, 4, 0,

		2, 3, 7,
		7, 6, 2,
	};

	private Framebuffer framebuffer;
	public Silk.NET.OpenGL.Texture renderTexture { get; private set; }

	private const int colorVal = 50;
	private Vector4D<int> clearColor = new(colorVal, colorVal, colorVal, 255);
	private Shader shader;
	private Texture texture;
	private IWindow window;

	public void Update(double deltaTime, Matrix4x4? view, Matrix4x4? projection)
	{
		unsafe
		{
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

			Gl.Enable(EnableCap.DepthTest);
			Gl.ClearColor(clearColor);
			Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

			vao.Bind();
			shader.Use();
			texture.Bind(TextureUnit.Texture0);
			shader.SetUniform("Texture0", 0);
			shader.SetUniform("UseTexture", false);

			Gl.DrawElements(PrimitiveType.Triangles, (uint) INDICES.Length, DrawElementsType.UnsignedInt, null);

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

	public void Resize(Vector2D<int> size) { }

	public void Load(IWindow window)
	{
		unsafe
		{
			Gl = GL.GetApi(window);
			this.window = window;
			ebo = new BufferObject<uint>(Gl, INDICES, BufferTargetARB.ElementArrayBuffer);
			vbo = new BufferObject<float>(Gl, VERTICES, BufferTargetARB.ArrayBuffer);
			vao = new VertexArrayObject<float, uint>(Gl, vbo, ebo);

			vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
			vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

			shader = new Shader(Gl, @"/resources/shaders/unlitvertex.glsl", @"/resources/shaders/unlitfragment.glsl");
			texture = new Texture(Gl, @"/resources/textures/test.png");
			Gl.GenFramebuffers(1, out framebuffer);
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer.Handle);

			Gl.GenTextures(1, out Silk.NET.OpenGL.Texture rt);
			renderTexture = rt;
			Gl.BindTexture(TextureTarget.Texture2D, renderTexture.Handle);
			Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, 3840, 2160, 0, PixelFormat.Rgba,
				PixelType.UnsignedByte, null);

			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int) TextureMinFilter.Linear);
			Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int) TextureMagFilter.Linear);

			Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
				TextureTarget.Texture2D, renderTexture.Handle, 0);

			var status = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status != GLEnum.FramebufferComplete)
			{
				Console.WriteLine($"Framebuffer is not complete! Status: {status}");
			}
		}
	}

	public void Close()
	{
		vbo.Dispose();
		ebo.Dispose();
		vao.Dispose();
		shader.Dispose();
		texture.Dispose();
	}


	public void SetRenderTargetSize(Vector2D<float> size)
	{
		unsafe
		{
			Gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgba, (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgba,
				PixelType.UnsignedByte, null);
		}
	}
}