using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;//for Type class
using System.Reflection;


[System.Serializable]
public class gameObjectSaveData
{
    public string name;
    public string prefabName;
    public string id;
    public string idParent;

    public bool active;
    public Vector3 position;
    public Vector3 localScale;
    public Quaternion rotation;

	public List<objectComponent> objectComponents = new List<objectComponent>();
    public gameObjectSaveData(GameObject original)
    {
		objectSaver oS = original.GetComponent<objectSaver>();
		name = original.name;
		prefabName = oS.prefabName;
		id = oS.id;
		if (original.transform.parent != null)
		{
			objectSaver parentOS = original.transform.parent.GetComponent<objectSaver>();
			if (parentOS != null)
				idParent = parentOS.id;
		}
		else
		{
			idParent = null;
		}

		//in this case, we will only store MonoBehavior scripts that are on the GO. The Transform is stored as part of the ScenObject isntance (assigned further down below).
		//If you wish to store other component types, you have to find you own ways to do it if the "easy" way that is used for storing components doesn't work for them.
		List<string> componentTypesToAdd = new List<string>() {
			"UnityEngine.MonoBehaviour"
		};

		//This list will hold only the components that are actually stored (MonoBehavior scripts, in this case)
		List<object> components_filtered = new List<object>();

		//Collect all the components that are attached to the GO.
		//This includes MonoBehavior scripts, Renderers, Transform, Animator...
		//If it
		object[] components_raw = original.GetComponents<Component>();
		foreach (object component_raw in components_raw)
		{
			if (componentTypesToAdd.Contains(component_raw.GetType().BaseType.FullName))
			{
				components_filtered.Add(component_raw);
			}
		}

		foreach (object component_filtered in components_filtered)
		{
			objectComponent  oC = PackComponent(component_filtered);
			if (oC != null)
				objectComponents.Add(oC);
		}

		//Assign all the GameObject's misc. values
		position = original.transform.position;
		localScale = original.transform.localScale;
		rotation = original.transform.rotation;
		active = original.activeSelf;

	}
	public objectComponent PackComponent(object component)
	{

		//This will go through all the fields of a component, check each one if it is serializable, and it it should be stored,
		//and puts it into the fields dictionary of a new instance of ObjectComponent,
		//with the field's name as key and the field's value as (object)value
		//for example, if a script has the field "float myFloat = 12.5f", then the key would be "myFloat" and the value "12.5f", tha latter stored as an object.

		objectComponent newObjectComponent = new objectComponent();
		newObjectComponent.fields = new Dictionary<string, object>();

		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
		var componentType = component.GetType();
		FieldInfo[] fields = componentType.GetFields(flags);

		newObjectComponent.componentName = componentType.ToString();

		foreach (FieldInfo field in fields)
		{

			if (field != null)
			{
				if (field.FieldType.IsSerializable == false)
				{
					//Debug.Log(field.Name + " (Type: " + field.FieldType + ") is not marked as serializable. Continue loop.");
					continue;
				}
				if (TypeSystem.IsEnumerableType(field.FieldType) == true || TypeSystem.IsCollectionType(field.FieldType) == true)
				{
					Type elementType = TypeSystem.GetElementType(field.FieldType);
					//Debug.Log(field.Name + " -> " + elementType);

					if (elementType.IsSerializable == false)
					{
						continue;
					}
				}
				newObjectComponent.saveableType = new(component.GetType());
				newObjectComponent.fields.Add(field.Name, field.GetValue(component));
				//Debug.Log(field.Name + " (Type: " + field.FieldType + "): " + field.GetValue(component));
			}
		}
		return newObjectComponent;
	}
	public void UnpackComponents(ref GameObject go, gameObjectSaveData sceneObject)
	{
		//Go through the stored object's component list and reassign all values in each component, and add components that are missing
		foreach (objectComponent obc in sceneObject.objectComponents)
		{
			if (obc.saveableType.SystemType == null)
            {
				continue;
            }
			if (go.GetComponent(obc.componentName) == false)
			{
				go.AddComponent(obc.saveableType.SystemType);
			}
			object obj = go.GetComponent(obc.saveableType.SystemType);

			var typ = obj.GetType();
			if (obc.fields != null)
			{
				foreach (KeyValuePair<string, object> p in obc.fields)
				{

					var fld = typ.GetField(p.Key,
										  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
										  BindingFlags.SetField);
					if (fld != null)
					{

						object value = p.Value;
						fld.SetValue(obj, value);
					}
				}
            }
		}
	}
}
