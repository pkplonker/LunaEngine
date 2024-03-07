namespace Engine;

public static class SceneController
{
	private static Scene? activeScene;

	public static Scene? ActiveScene
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
	public static Action<Scene?, Scene?> OnActiveSceneChanged { get; set; }
}