using System.Drawing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public class Renderer
{
	private static GL gl;

	private static uint Vbo;
	private static uint Ebo;
	private static uint Vao;

	//Vertex data, uploaded to the VBO.
	private static readonly float[] Vertices =
	{
		//X    Y      Z
		0.5f, 0.5f, 0.0f,
		0.5f, -0.5f, 0.0f,
		-0.5f, -0.5f, 0.0f,
		-0.5f, 0.5f, 0.5f
	};

	//Index data, uploaded to the EBO.
	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	private Vector4D<int> clearColor = new(18, 18, 18, 255);
	private Shader shader;

	public void Update(double deltaTime)
	{
		unsafe
		{
			gl.ClearColor(clearColor);
			gl.Clear((uint) ClearBufferMask.ColorBufferBit);

			gl.BindVertexArray(Vao);
			shader.Use();

			gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
		}
	}

	public void Resize(Vector2D<int> size) { }

	public void Load(IWindow window)
	{
		unsafe
		{
			gl = GL.GetApi(window);

			Vao = gl.GenVertexArray();
			gl.BindVertexArray(Vao);

			Vbo = gl.GenBuffer();
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);
			fixed (void* v = &Vertices[0])
			{
				gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (Vertices.Length * sizeof(uint)), v,
					BufferUsageARB.StaticDraw);
			}

			Ebo = gl.GenBuffer();
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo);
			fixed (void* i = &Indices[0])
			{
				gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (Indices.Length * sizeof(uint)), i,
					BufferUsageARB.StaticDraw); //Setting buffer data.
			}

			shader = new Shader(gl, @"/resources/shaders/unlitvertex.glsl", @"/resources/shaders/unlitfragment.glsl");

			gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
			gl.EnableVertexAttribArray(0);
		}
	}

	public void Close()
	{
		gl.DeleteBuffer(Vbo);
		gl.DeleteBuffer(Ebo);
		gl.DeleteVertexArray(Vao);
	}
}