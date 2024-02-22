using System.Diagnostics;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class Shader : IDisposable
{
	private uint shaderID;
	private GL gl;
	private Dictionary<string, int> uniformDict = new();

	public Shader(GL gl, string vertexPath, string fragmentPath)
	{
		this.gl = gl;
		uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
		uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
		shaderID = this.gl.CreateProgram();
		this.gl.AttachShader(shaderID, vertex);
		this.gl.AttachShader(shaderID, fragment);
		this.gl.LinkProgram(shaderID);
		this.gl.GetProgram(shaderID, GLEnum.LinkStatus, out var status);
		if (status == 0)
		{
			throw new Exception($"Program failed to link with error: {this.gl.GetProgramInfoLog(shaderID)}");
		}

		this.gl.DetachShader(shaderID, vertex);
		this.gl.DetachShader(shaderID, fragment);
		this.gl.DeleteShader(vertex);
		this.gl.DeleteShader(fragment);
	}

	public void Use()
	{
		gl.UseProgram(shaderID);
	}

	private bool TryGetUniformLocation(string name, out int location)
	{
		if (!uniformDict.ContainsKey(name))
		{
			location = gl.GetUniformLocation(shaderID, name);
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

	public void Dispose()
	{
		gl.DeleteProgram(shaderID);
	}

	private uint LoadShader(ShaderType type, string path)
	{
		path = path.MakeAbsolute();
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
}