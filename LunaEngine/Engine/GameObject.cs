﻿using System.Collections;
using System.Diagnostics;
using Debug = Engine.Logging.Debug;

namespace Engine;

[Inspectable(false)]
[Serializable]
public class GameObject
{
	public string Name = "DEFAULT_NAME";
	private HashSet<IComponent> components = new();
	[Serializable(false)]
	public Transform Transform { get; private set; }
	public bool Enabled = true;

	public GameObject()
	{
		Transform = new Transform(this);
	}

	public T? GetComponent<T>() where T : class?, IComponent
	{
		return components?.FirstOrDefault(x => x is T) as T ?? null;
	}

	public bool TryGetComponent<T>(out IComponent? component) where T : class?, IComponent
	{
		component = GetComponent<T>();
		return component != null;
	}

	public T? AddComponent<T>() where T : class?, IComponent
	{
		T component = default(T);

		if (!typeof(T).GetInterfaces().Where(x => x == typeof(IComponent)).Any())
		{
			return component;
		}

		var constructor = typeof(T).GetConstructor(new[] {typeof(GameObject)});
		if (constructor != null)
		{
			component = (T) constructor.Invoke(new object[] {this});
		}

		if (component != null)
		{
			components.Add(component);
		}

		return component;
	}

	public void RemoveComponent<T>() where T : Component
	{
		var component = components.FirstOrDefault(c => c.GetType() == typeof(T));

		if (component != null)
		{
			components.Remove(component);
		}
	}

	public void RemoveComponent(Component? component)
	{
		if (components.Contains(component))
		{
			components.Remove(component);
		}
	}

	public void Update()
	{
		if (!Enabled) return;
		foreach (var component in components)
		{
			component.Update();
		}
	}

	public HashSet<IComponent> GetComponents() => components;
}