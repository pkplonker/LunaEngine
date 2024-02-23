using System.Drawing;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public class Renderer
{
	private static GL gl;

	private static BufferObject<float> vbo;
	private static BufferObject<uint> ebo;
	private static VertexArrayObject<float, uint> vao;

	private static readonly float[] VERTICES =
	{
		//X    Y      Z     U   V
		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
		0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f, -0.5f,  0.0f, 0.0f,

		-0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
		0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
		0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
	};

	private static readonly uint[] INDICES =
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
	


	private Vector4D<int> clearColor = new(18, 18, 18, 255);
	private Shader shader;
	private texture texture;
	private Camera camera;
	private IWindow window;

	public void Update(double deltaTime)
	{
		unsafe
		{
			gl.Enable(EnableCap.DepthTest);
			gl.ClearColor(clearColor);
			gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

			vao.Bind();
			shader.Use();
			texture.Bind(TextureUnit.Texture0);

			shader.SetUniform("Texture0", 0);
			shader.SetUniform("UseTexture", false);

			gl.DrawElements(PrimitiveType.Triangles, (uint) INDICES.Length, DrawElementsType.UnsignedInt, null);
		}

		camera.OnUpdate(deltaTime);

		var difference = (float) (window.Time * 100);

		var model = Matrix4x4.CreateRotationY(MathExtensions.DegreesToRadians(difference)) *
		            Matrix4x4.CreateRotationX(MathExtensions.DegreesToRadians(difference));
		var view = camera.GetView();
		var projection = camera.GetProjection();
		shader.SetUniform("uModel", model);
		shader.SetUniform("uView", view);
		shader.SetUniform("uProjection", projection);
	}

	public void Resize(Vector2D<int> size) { }

	public void Load(IWindow window)
	{
		unsafe
		{
			gl = GL.GetApi(window);
			this.window = window;
			ebo = new BufferObject<uint>(gl, INDICES, BufferTargetARB.ElementArrayBuffer);
			vbo = new BufferObject<float>(gl, VERTICES, BufferTargetARB.ArrayBuffer);
			vao = new VertexArrayObject<float, uint>(gl, vbo, ebo);

			vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
			vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

			shader = new Shader(gl, @"/resources/shaders/unlitvertex.glsl", @"/resources/shaders/unlitfragment.glsl");
			texture = new texture(gl, @"/resources/textures/test.png");
		}

		IInputContext input = window.CreateInput();
		camera = new Camera(input);
	}

	public void Close()
	{
		vbo.Dispose();
		ebo.Dispose();
		vao.Dispose();
		shader.Dispose();
		texture.Dispose();
	}
}