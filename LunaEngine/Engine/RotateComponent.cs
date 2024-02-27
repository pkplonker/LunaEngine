using System.Numerics;

namespace Engine;

public class RotateComponent : Component
{
	[Serializable]
	private float rotationAmount = 1;
	public RotateComponent(GameObject gameObject) : base(gameObject) { }

	public override void Update()
	{
		float amount = rotationAmount*Time.DeltaTime;
		GameObject.Transform.RotateByEuler(new Vector3(amount, amount, amount));
	}
}