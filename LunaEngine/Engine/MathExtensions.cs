namespace Engine;

public static class MathExtensions
{
	public static float DegreesToRadians(float degrees)
	{
		return MathF.PI / 180f * degrees;
	}
}