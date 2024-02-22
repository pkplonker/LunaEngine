using System.Drawing;
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

	private static readonly float[] Vertices =
	{
		//X    Y      Z     S    T
		0.5f, 0.5f, 0.0f, 1.0f, 0.0f,
		0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
		-0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
		-0.5f, 0.5f, 0.5f, 0.0f, 0.0f
	};

	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	private Vector4D<int> clearColor = new(18, 18, 18, 255);
	private Shader shader;
	private texture texture;

	public void Update(double deltaTime)
	{
		unsafe
		{
			gl.ClearColor(clearColor);
			gl.Clear((uint) ClearBufferMask.ColorBufferBit);

			vao.Bind();
			shader.Use();
			texture.Bind(TextureUnit.Texture0);

			//Setting a uniform.
			shader.SetUniform("Texture0", 0);
			shader.SetUniform("UseTexture", true);

			gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
		}
	}

	public void Resize(Vector2D<int> size) { }

	public void Load(IWindow window)
	{
		unsafe
		{
			gl = GL.GetApi(window);

			//Instantiating our new abstractions
			ebo = new BufferObject<uint>(gl, Indices, BufferTargetARB.ElementArrayBuffer);
			vbo = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
			vao = new VertexArrayObject<float, uint>(gl, vbo, ebo);

			//Telling the VAO object how to lay out the attribute pointers
			vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
			vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

			shader = new Shader(gl, @"/resources/shaders/unlitvertex.glsl", @"/resources/shaders/unlitfragment.glsl");
			texture = new texture(gl, @"/resources/textures/test.png");
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