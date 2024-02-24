using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class Mesh : IDisposable, IRenderable
{
	public Mesh(GL gl, float[] vertices, uint[] indices)
	{
		GL = gl;
		Vertices = vertices;
		Indices = indices;
		SetupMesh();
	}

	public float[] Vertices { get; private set; }
	public uint[] Indices { get; private set; }
	public VertexArrayObject<float, uint> VAO { get; set; }
	public BufferObject<float> VBO { get; set; }
	public BufferObject<uint> EBO { get; set; }
	public GL GL { get; }

	public unsafe void SetupMesh()
	{
		EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
		VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
		VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
		VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
		VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
	}
	public unsafe void Render(Renderer renderer, RenderPassData data)
	{
		VAO.Bind();
		renderer.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt);
	}


	public void Dispose()
	{
		VAO.Dispose();
		VBO.Dispose();
		EBO.Dispose();
	}
}