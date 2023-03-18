using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Add an ObjectSaver component to each Prefab that might possibly be serialized and deserialized.
public class objectSaver : MonoBehaviour {
	
	public string instanceName;
	//The name variable is not used by the serialization; it is just there so you can name your prefabs any way you want, 
	//while the "in-game" name can be something different
	//A GameObject's (and thus, prefab's) name should be the same as prefabName,
	////while the variable "instanceName" in this script can be anything (or nothing at all).
	public string prefabName;

	public string id;
	public string idParent;
	public bool dontSave = false;
	public bool isEnemy = false;

	public GameObject EnemyWeaponSave;
	public void SetID() {
		
		id = System.Guid.NewGuid().ToString();
		CheckForRelatives();
	}
	
	private void CheckForRelatives() {
		
		if(transform.parent == null) {
			idParent = null;
		}
		else {
			objectSaver[] childrenIds = GetComponentsInChildren<objectSaver>();
			foreach(objectSaver idScript in childrenIds) {
				if(idScript.transform.gameObject != gameObject) {
					idScript.idParent = id;
					idScript.SetID();
				}
			}
		}
	}

	public void OnSerialize()
	{
		saveManager.SaveDataEvent += Save;
	}
	private void OnDisable()

	{
		saveManager.SaveDataEvent -= Save;
		dontSave = true;
	}
	public void Save(saveData data)
	{
		//creates a new serializable data from the attached gameobject
		gameObjectSaveData newObjectData;
		if (gameObject.tag == "Player")
        {
			newObjectData = new gameObjectSaveData(gameObject, true);

		} else
        {
			newObjectData = new gameObjectSaveData(gameObject);
		}

		newObjectData.isEnemy = isEnemy;
		if (isEnemy)
        {
			newObjectData.EnemyWeaponSave = EnemyWeaponSave;
        }
		data.objectsData.Add(newObjectData);
	}
}

