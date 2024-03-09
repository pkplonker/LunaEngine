namespace Engine;

public static class SceneController
{
	private static IScene? activeScene;

	public static IScene? ActiveScene
	{
		get => activeScene;
		set
		{
			if (value != activeScene)
			{
				var oldScene = activeScene;
				activeScene = value;
				OnActiveSceneChanged?.Invoke(activeScene,oldScene);
			}
		} }
	public static Action<IScene?, IScene?> OnActiveSceneChanged { get; set; }
}