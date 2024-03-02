using System.Diagnostics;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Serializable]
public class Shader : IDisposable
{
	private uint handle;
	private GL gl;
	private Dictionary<string, int> uniformDict = new();
	public string vertexPath { get; private set; }
	public string fragmentPath { get; private set; }
	[Serializable(false)]
	public Guid guid { get; private set; } = Guid.NewGuid();

	public Shader(GL gl, string vertexPath, string fragmentPath)
	{
		this.fragmentPath = fragmentPath;
		this.vertexPath = vertexPath;
		this.gl = gl;

		uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
		uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);

		handle = this.gl.CreateProgram();
		this.gl.AttachShader(handle, vertex);
		this.gl.AttachShader(handle, fragment);
		this.gl.LinkProgram(handle);
		this.gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
		if (status == 0)
		{
			throw new Exception($"Program failed to link with error: {this.gl.GetProgramInfoLog(handle)}");
		}

		this.gl.DetachShader(handle, vertex);
		this.gl.DetachShader(handle, fragment);
		this.gl.DeleteShader(vertex);
		this.gl.DeleteShader(fragment);
	}

	public void Use()
	{
		gl.UseProgram(handle);
	}

	private bool TryGetUniformLocation(string name, out int location)
	{
		if (!uniformDict.ContainsKey(name))
		{
			location = gl.GetUniformLocation(handle, name);
			if (location == -1)
			{
				Debug.WriteLine($"{name} uniform not found on shader.");
				return false;
			}

			uniformDict[name] = location;
		}

		location = uniformDict[name];
		return true;
	}

	public void SetUniform(string name, int value)
	{
		if (!TryGetUniformLocation(name, out var location))
		{
			throw new Exception($"{name} uniform not found on shader.");
		}

		gl.Uniform1(location, value);
	}

	public unsafe void SetUniform(string name, Matrix4x4 value)
	{
		if (!TryGetUniformLocation(name, out var location))
		{
			throw new Exception($"{name} uniform not found on shader.");
		}

		gl.UniformMatrix4(location, 1, false, (float*) &value);
	}

	public void SetUniform(string name, float value)
	{
		if (!TryGetUniformLocation(name, out var location))
		{
			throw new Exception($"{name} uniform not found on shader.");
		}

		gl.Uniform1(location, value);
	}

	public void SetUniform(string name, bool value)
	{
		if (!TryGetUniformLocation(name, out var location))
		{
			throw new Exception($"{name} uniform not found on shader.");
		}

		gl.Uniform1(location, value ? 1 : 0);
	}

	private uint LoadShader(ShaderType type, string path)
	{
		if (!File.Exists(path))
		{
			throw new FileNotFoundException(path);
		}

		string src = File.ReadAllText(path);
		uint handle = gl.CreateShader(type);
		gl.ShaderSource(handle, src);
		gl.CompileShader(handle);
		string infoLog = gl.GetShaderInfoLog(handle);
		if (!string.IsNullOrWhiteSpace(infoLog))
		{
			throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
		}

		return handle;
	}

	public void Dispose()
	{
		gl.DeleteProgram(handle);
	}
}