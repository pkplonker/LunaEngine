using System.Numerics;
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
		-0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
		0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
		0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f, -0.5f,  0.0f, 0.0f,

		-0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
		0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
		0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
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


	private const int colorVal = 50;
	private Vector4D<int> clearColor = new(colorVal,colorVal,colorVal, 255);
	private Shader shader;
	private Texture texture;
	private IWindow window;

	public void Update(double deltaTime, Matrix4x4? view, Matrix4x4? projection)
	{
		unsafe
		{
			Gl.Enable(EnableCap.DepthTest);
			Gl.ClearColor(clearColor);
			Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

			vao.Bind();
			shader.Use();
			texture.Bind(TextureUnit.Texture0);
			
			shader.SetUniform("Texture0", 0);
			shader.SetUniform("UseTexture", false);

			Gl.DrawElements(PrimitiveType.Triangles, (uint) INDICES.Length, DrawElementsType.UnsignedInt, null);
		}


		var difference = (float) (window.Time * 100);

		var model = Matrix4x4.CreateRotationY(MathExtensions.DegreesToRadians(difference)) *
		            Matrix4x4.CreateRotationX(MathExtensions.DegreesToRadians(difference));
		shader.SetUniform("uModel", model);
		
		if(view.HasValue)
			shader.SetUniform("uView", view.Value);
		if(projection.HasValue)
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
}