using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class Mesh : IDisposable
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
	private const int vertexSize = 14;

	public unsafe void SetupMesh()
	{
		EBO = new BufferObject<uint>(GL, Indices, BufferTargetARB.ElementArrayBuffer);
		VBO = new BufferObject<float>(GL, Vertices, BufferTargetARB.ArrayBuffer);
		VAO = new VertexArrayObject<float, uint>(GL, VBO, EBO);
		VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, vertexSize, 0);
		VAO.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, vertexSize, 12);
		VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, vertexSize, 24);
		VAO.VertexAttributePointer(3, 2, VertexAttribPointerType.Float, vertexSize, 36);
		VAO.VertexAttributePointer(4, 3, VertexAttribPointerType.Float, vertexSize, 44);
		VAO.UnBind();
	}

	public void Render(Renderer renderer, RenderPassData data)
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