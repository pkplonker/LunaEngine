using System.Numerics;

namespace Engine;

public class RotateComponent : Component
{
	public RotateComponent(GameObject gameObject) : base(gameObject) { }
	private const float rotationAmount = 1;
	public override void Update()
	{
		float amount = rotationAmount*Time.DeltaTime;
		GameObject.Transform.RotateByEuler(new Vector3(amount, amount, amount));
	}
}