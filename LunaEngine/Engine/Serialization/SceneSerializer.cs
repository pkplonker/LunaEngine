﻿using System.Collections;
using System.Reflection;
using Engine.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Editor.Properties;

namespace Engine
{
	public class SceneSerializer
	{
		private readonly Scene scene;
		private readonly string absolutePath;
		private JObject rootObject;

		public SceneSerializer(Scene scene, string absolutePath)
		{
			this.scene = scene;
			this.absolutePath = absolutePath;
			this.rootObject = new JObject();
			serializer = new JsonSerializer
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				Converters = {new GuidEnumerableConverter()}
			};
		}

		public bool Serialize()
		{
			bool result = true;
			try
			{
				foreach (var go in scene?.ChildrenAsGameObjectsRecursive)
				{
					SerializeGameObject(go);
				}

				File.WriteAllText(absolutePath, rootObject.ToString());
			}
			catch (Exception e)
			{
				Logger.Error(e);
				result = false;
			}

			if (result)
			{
				Logger.Log($"Serialized scene to {absolutePath}");
			}
			else
			{
				Logger.Warning($"Failed serializing scene to {absolutePath}");
			}

			return result;
		}

		private void SerializeGameObject(GameObject? go)
		{
			if (go == null) return;
			var gameObjectJObject = new JObject();
			var componentsJObject = new JObject();
			SerializeTransform(go.Transform, gameObjectJObject);
			SerializeProperties(go, gameObjectJObject);

			var components = go.GetComponents();
			foreach (var component in components)
			{
				SerializeComponent(component, componentsJObject);
			}

			if (componentsJObject.Count > 0)
			{
				gameObjectJObject.Add("components", componentsJObject);
			}

			rootObject.Add(go.Name, gameObjectJObject);
		}

		private void SerializeTransform(Transform transform, JObject parentObject)
		{
			var transformJObject = new JObject();
			SerializeProperties(transform, transformJObject);
			parentObject.Add("transform", transformJObject);
		}

		private void SerializeComponent(IComponent component, JObject parentObject)
		{
			var componentJObject = new JObject();
			SerializeProperties(component, componentJObject);
			parentObject.Add(component.GetType().AssemblyQualifiedName, componentJObject);
		}

		private BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private JsonSerializer serializer;

		private void SerializeProperties(object obj, JObject parentObject)
		{
			try
			{
				var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

				var properties = obj.GetType().GetProperties(flags)
					.Where(prop =>
					{
						var attr = prop.GetCustomAttribute<SerializableAttribute>();
						if (attr != null)
						{
							return attr.Show;
						}

						return prop.CanRead && prop.CanWrite;
					});

				foreach (var property in properties)
				{
					CreateObjectFromProperty(obj, parentObject, new PropertyAdapter(property));
				}

				var fields = obj.GetType().GetFields(flags)
					.Where(field => field.IsDefined(typeof(SerializableAttribute), true) &&
					                field.GetCustomAttribute<SerializableAttribute>().Show);

				foreach (var field in fields)
				{
					CreateObjectFromProperty(obj, parentObject, new FieldAdapter(field));
				}
			}
			catch (Exception e)
			{
				Logger.Warning($"Err {obj.GetType()}");
			}
		}

		private void CreateObjectFromProperty(object obj, JObject parentObject, IMemberAdapter member)
		{
			var value = member.GetValue(obj);

			var attribute = value?.GetType().GetCustomAttribute<SerializableAttribute>();
			var resourceAttribute = value?.GetType().GetCustomAttribute<ResourceIdentifierAttribute>();
			if (value != null && resourceAttribute != null)
			{
				var guidProperty = member.MemberType.GetProperty("GUID");
				if (guidProperty != null)
				{
					var guidValue = guidProperty.GetValue(value);
					parentObject.Add(member.Name, JToken.FromObject(guidValue, serializer));
				}
				else
				{
					Logger.Warning($"GUID property null when serializing");
					return;
				}
			}
			else if (value != null && attribute != null && attribute.Show)
			{
				var customObjectJObject = new JObject();
				SerializeProperties(value, customObjectJObject);
				parentObject.Add(member.Name, customObjectJObject);
			}
			else if (IsCollection(member.MemberType))
			{
				if (value is IEnumerable collection)
				{
					var array = new JArray();
					foreach (var element in collection)
					{
						var elementType = element.GetType();
						var resourceAttri = elementType.GetCustomAttribute<ResourceIdentifierAttribute>();

						if (resourceAttri != null)
						{
							var guidProperty = elementType.GetProperty("GUID");
							if (guidProperty != null)
							{
								var guidValue = guidProperty.GetValue(element);
								array.Add(JToken.FromObject(guidValue, serializer));
							}
							else
							{
								Logger.Warning(
									$"GUID property not found for element in collection marked with ResourceIdentifierAttribute.");
							}
						}
						else
						{
							var elementObject = new JObject();
							SerializeProperties(element, elementObject);
							array.Add(elementObject);
						}
					}

					parentObject.Add(member.Name, array);
				}
			}
			else
			{
				parentObject.Add(member.Name, JToken.FromObject(value, serializer));
			}
		}

		private bool IsCollection(Type type) => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
	}
}