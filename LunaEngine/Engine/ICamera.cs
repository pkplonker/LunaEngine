using System.Numerics;

namespace Engine;

public interface ICamera
{
	Matrix4x4 GetView();
	Matrix4x4 GetProjection();
}