using System.Collections.Generic;
//The ObjectComponent class holds all data of a gameobject's component.
[System.Serializable]
public class objectComponent {
	public string componentName;
	public serializableType saveableType;
	//The Dictionary holds the actual data of a component; A field's name as key and the corresponding value (object) as value. Confusing, right?
	public Dictionary<string,object> fields;
}