using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Engine.Logging;

namespace Engine
{
	public class SceneDeserializer
	{
		private readonly string absolutePath;
		private readonly JsonSerializer serializer;
		private Dictionary<Guid, GameObject> gameObjectLookup;

		public SceneDeserializer(string absolutePath)
		{
			this.absolutePath = absolutePath;
			serializer = new JsonSerializer
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				Converters = {new GuidEnumerableConverter()}
			};
			gameObjectLookup = new Dictionary<Guid, GameObject>();
		}

		public Scene? Deserialize()
		{
			try
			{
				string json = File.ReadAllText(absolutePath);
				JObject rootObject = JObject.Parse(json);
				Scene scene = new Scene();

				foreach (var goToken in rootObject)
				{
					GameObject go = DeserializeGameObject(goToken.Key, goToken.Value as JObject);
					if (go != null)
					{
						gameObjectLookup.Add(go.Transform.GUID, go);
						scene.AddGameObject(go);
					}
				}

				foreach (var go in gameObjectLookup.Values)
				{
					var childrenGuids = go.Transform.ChildrenGuids; 
					foreach (var childGuid in childrenGuids)
					{
						if (gameObjectLookup.TryGetValue(childGuid, out var child))
						{
							child.Transform.SetParent(go.Transform);
						}
					}
				}

				return scene;
			}
			catch (Exception e)
			{
				Logger.Error($"Failed to deserialize {absolutePath} - {e}");
				return null;
			}
		}

		private GameObject DeserializeGameObject(string name, JObject gameObjectJObject)
		{
			var go = new GameObject
			{
				Name = name
			};

			var transformObject = gameObjectJObject["transform"] as JObject;
			if (transformObject != null)
			{
				DeserializeProperties(go.Transform, transformObject);
			}

			var componentsObject = gameObjectJObject["components"] as JObject;
			if (componentsObject != null)
			{
				foreach (var component in componentsObject)
				{
					var componentType = Type.GetType(component.Key);
					if (componentType != null)
					{
						DeserializeComponent(componentType, component.Value as JObject, go);
					}
				}
			}

			return go;
		}

		private void DeserializeProperties(object obj, JObject parentObject)
		{
			foreach (var prop in parentObject.Properties())
			{
				var propertyInfo = obj.GetType().GetProperty(prop.Name);
				if (propertyInfo != null && propertyInfo.CanWrite)
				{
					if (Attribute.IsDefined(propertyInfo.PropertyType, typeof(ResourceIdentifierAttribute)))
					{
						if (prop.Value.Type == JTokenType.String)
						{
							Guid guid;
							if (Guid.TryParse(prop.Value.ToString(), out guid))
							{
								var resource = ResourceManager.GetResourceByGuid(propertyInfo.PropertyType, guid);
								propertyInfo.SetValue(obj, resource);
							}
							else
							{
								Logger.Warning($"Invalid GUID format for property '{prop.Name}'");
							}
						}
					}
					else if (IsCollection(propertyInfo.PropertyType) && prop.Value is JArray array)
					{
						Type collectionType = propertyInfo.PropertyType;
						Type elementType = collectionType.GetGenericArguments()[0];

						var collectionInstance = Activator.CreateInstance(collectionType);

						foreach (var elementToken in array)
						{
							object elementObj = null;

							if (Attribute.IsDefined(elementType, typeof(ResourceIdentifierAttribute)))
							{
								if (elementToken.Type == JTokenType.String)
								{
									Guid guid;
									if (Guid.TryParse(elementToken.ToString(), out guid))
									{
										elementObj = ResourceManager.GetResourceByGuid(elementType, guid);
									}
									else
									{
										Logger.Warning($"Invalid GUID format for property '{elementType.Name}'");
									}
								}
								else
								{
									Logger.Warning(
										$"Expected a GUID for resource identifier, but got: {elementToken.Type}");
									continue;
								}
							}
							else
							{
								elementObj = Activator.CreateInstance(elementType);
								DeserializeProperties(elementObj, elementToken as JObject);
							}

							if (collectionInstance is IList list)
							{
								list.Add(elementObj);
							}
							else if (collectionInstance is ISet<object> set)
							{
								set.Add(elementObj);
							}
						}

						propertyInfo.SetValue(obj, collectionInstance);
					}
					else
					{
						try
						{
							var value = prop.Value.ToObject(propertyInfo.PropertyType, serializer);
							propertyInfo.SetValue(obj, value);
						}
						catch (Exception e)
						{
							Logger.Warning("err");
						}
					}
				}
			}
		}

		private bool IsCollection(Type type)
		{
			return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
		}

		private void DeserializeComponent(Type componentType, JObject componentJObject, GameObject go)
		{
			var constructor = componentType.GetConstructor(new[] {typeof(GameObject)});
			if (constructor == null)
			{
				Logger.Error($"No suitable constructor found for {componentType.Name}");
				return;
			}

			var component = (IComponent) constructor.Invoke(new object[] {go});
			if (component != null)
			{
				DeserializeProperties(component, componentJObject);
				go.AddComponent(component);
			}
		}
	}
}