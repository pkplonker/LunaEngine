using System.Numerics;

namespace Engine;

public class RotateComponent : Component
{
	public RotateComponent(GameObject gameObject) : base(gameObject) { }
	private const float rotationAmount = 10f;
	public override void Update(double deltaTime)
	{
		float amount = rotationAmount*(float)deltaTime;
		GameObject.Transform.RotateByEuler(new Vector3(amount, amount, amount));
	}
}