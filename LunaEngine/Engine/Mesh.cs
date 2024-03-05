using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Serializable]
[ResourceIdentifier]
public class Mesh : IDisposable
{
	public Mesh(GL gl, float[] vertices, uint[] indices, Guid metadataGuid)
	{
		this.GUID = metadataGuid;
		GL = gl;
		Vertices = vertices;
		Indices = indices;
		SetupMesh();
	}

	[Serializable(false)]
	public float[] Vertices { get; private set; }

	[Serializable(false)]

	public uint[] Indices { get; private set; }

	[Serializable(false)]

	public VertexArrayObject<float, uint> VAO { get; set; }

	[Serializable(false)]

	public BufferObject<float> VBO { get; set; }

	[Serializable(false)]

	public BufferObject<uint> EBO { get; set; }

	public GL GL { get; }

	[Serializable(false)]
	private const int vertexSize = 14;

	[Inspectable(false)]
	[Serializable]
	public Guid GUID { get; set; } = Guid.NewGuid();

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