using System.Collections;
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
				Debug.Error(e);
				result = false;
			}

			return result;
		}

		private void SerializeGameObject(GameObject? go)
		{
			if (go == null) return;
			var gameObjectJObject = new JObject();
			var components = go.GetComponents();
			SerializeTransform(go.Transform, gameObjectJObject);
			SerializeProperties(go, gameObjectJObject);
			foreach (var component in components)
			{
				SerializeComponent(component, gameObjectJObject);
			}

			rootObject.Add(go.Name, gameObjectJObject);
		}

		private void SerializeTransform(Transform transform, JObject parentObject)
		{
			SerializeProperties(transform, parentObject);
		}

		private void SerializeComponent(IComponent component, JObject parentObject)
		{
			var componentJObject = new JObject();
			SerializeProperties(component, componentJObject);
			parentObject.Add(component.GetType().Name, componentJObject);
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
				Debug.Warning($"Err {obj.GetType()}");
			}
		}

		private void CreateObjectFromProperty(object obj, JObject parentObject, IMemberAdapter member)
		{
			var value = member.GetValue(obj);

			var attribute = value?.GetType().GetCustomAttribute<SerializableAttribute>();

			if (value != null && attribute != null && attribute.Show)
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
						var elementObject = new JObject();
						SerializeProperties(element, elementObject);
						array.Add(elementObject);
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