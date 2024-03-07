namespace Engine;

public static class LinqExtensions
{
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
	{
		return source.Where(item => item != null);
	}

	public static void Foreach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			if (item == null) continue;
			action(item);
		}
	}
}