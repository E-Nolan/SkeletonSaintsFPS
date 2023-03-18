using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveManagerInterface : MonoBehaviour
{
	public Dictionary<string, GameObject> prefabDictionary;
	public string defaultSaveName = "Main Save";

	public static saveManagerInterface instance;

    private void Awake()
    {
		instance = this;
    }

    void Start()
	{
		if (saveManager.saveHelper == null)
		{
			saveManager.saveHelper = this;
		}
		prefabDictionary = new Dictionary<string, GameObject>();
		GameObject[] prefabs = Resources.LoadAll<GameObject>("");
		foreach (GameObject loadedPrefab in prefabs)
		{
			if (loadedPrefab.GetComponent<objectSaver>())
			{
				prefabDictionary.Add(loadedPrefab.name, loadedPrefab);
				////Debug.Log("Added GameObject to prefabDictionary: " + loadedPrefab.name);
			}
		}
	}

	public void SaveMain()
	{
		saveManager.mainData_Current.Index++;
		saveManager.saveHelper.SaveGame(defaultSaveName);
		Debug.Log(saveManager.mainData_Current.ToString());
	}
	//if 0 it will load the most recent save.
	public void LoadMain()
	{
		saveManager.saveHelper.LoadGame(defaultSaveName);

	}
	private void SaveGame(string gameName)
	{

		if (string.IsNullOrEmpty(gameName))
		{
			//Debug.Log("SaveGameName is null or empty!");
			return;
		}

		//Create a new instance of SaveGame. This will hold all the data that should be saved in our scene.
		saveData newSaveGame = new saveData();
		newSaveGame.name = gameName;

		List<GameObject> goList = new List<GameObject>();

		//Find all objectSaver components in the scene.
		//Since we can access the gameObject to which each one belongs with .gameObject, we thereby get all GameObject in the scene which should be saved!
		objectSaver[] objectsToSerialize = FindObjectsOfType<objectSaver>();

		if (objectsToSerialize != null)
		{
			//Go through the "raw" collection of components
			foreach (objectSaver saver in objectsToSerialize)
			{
				//First, we will set the ID of the GO if it doesn't already have one.
				if (string.IsNullOrEmpty(saver.id) == true)
				{
					saver.SetID();
				}
				goList.Add(saver.gameObject);
			}
			//This is a good time to call any functions on the GO that should be called before it gets serialized as part of a SaveGame. Example below.
			foreach (GameObject go in goList)
			{
				go.SendMessage("OnSerialize", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			//Debug.Log("No saveableobjects found");
		}

		//invoke the save event
		saveManager.TriggerSerialization(newSaveGame);
		//Call the static method that serialized our game and writes the data to a file.
		saveManager.SaveGameData(newSaveGame);
	}
	private void LoadGame(string gameName)
	{

		//First, we will destroy all objects in the scene which are not tagged with "DontDestroy" (such as Cameras, Managers of any type, and so on... things that should persist)
		ClearScene();

		//Call the static method that will attempt to load the specified file and deserialize it's data into a form that we can use
		saveData loadedGame = saveManager.LoadGameData(saveManager.directory + gameName);
		if (loadedGame == null)
		{
			//Debug.Log("saveGameName " + gameName + "couldn't be found!");
			return;
		}

		//create a new list that will hold all the gameObjects we will create anew from the deserialized data
		List<GameObject> goList = new List<GameObject>();

		//iterate through the loaded game's sceneObjects list to access each stored objet's data and reconstruct it with all it's components
		foreach (gameObjectSaveData loadedObject in loadedGame.objectsData)
		{
			GameObject go_reconstructed;
			go_reconstructed = UnpackGameObject(loadedObject);
			if (go_reconstructed != null)
			{
				//Add the reconstructed GO to the list we created earlier.
				goList.Add(go_reconstructed);
			}
		}

		//Go through the list of reconstructed GOs and reassign any missing children
		foreach (GameObject go in goList)
		{
			string parentId = go.GetComponent<objectSaver>().idParent;
			if (string.IsNullOrEmpty(parentId) == false)
			{
				foreach (GameObject go_parent in goList)
				{
					if (go_parent.GetComponent<objectSaver>().id == parentId)
					{
						go.transform.parent = go_parent.transform;
					}
				}
			}
		}

		//This is when you might want to call any functions that should be called when a gameobject is loaded. Example below.
		foreach (GameObject go2 in goList)
		{
			go2.SendMessage("OnDeserialize", SendMessageOptions.DontRequireReceiver);
		}

	}
	private static void ClearScene()
	{
		gameManager.instance.SetPlayerController = null;
		gameManager.instance.SetCameraControls = null;
		gameManager.instance.playerInstance = null;
		objectSaver[] allSaveableObjects = FindObjectsOfType<objectSaver>();
		if (allSaveableObjects.Length > 0)
		{
			foreach (objectSaver oS in allSaveableObjects)
			{
				Destroy(oS.gameObject);
			}
		}
	}

	public GameObject UnpackGameObject(gameObjectSaveData sceneObject)
	{
		//This is where our prefabDictionary above comes in. Each GameObject that was saved needs to be reconstucted, so we need a Prefab,
		//and we know which prefab it is because we stored the GameObject's prefab name in it's objectSaver/gameObjectSaveData script/class.
		//Theoretically, we could even reconstruct GO's that have no prefab by instatiating an empty GO and filling it with the required components
		if (prefabDictionary.ContainsKey(sceneObject.prefabName) == false)
		{
			//Debug.Log("Can't find key " + sceneObject.prefabName + " in SaveLoadMenu.prefabDictionary!");
			return null;
		}
		GameObject go = Instantiate(prefabDictionary[sceneObject.prefabName], sceneObject.position, sceneObject.rotation);

		//Reassign values
		go.name = sceneObject.name;
		go.transform.localScale = sceneObject.localScale;
		go.SetActive(sceneObject.active);

		objectSaver idScript = go.GetComponent<objectSaver>();
		idScript.id = sceneObject.id;
		idScript.idParent = sceneObject.idParent;

		sceneObject.UnpackComponents(ref go, sceneObject);



		//Destroy any children that were not referenced as having a parent
		objectSaver[] childrenIds = go.GetComponentsInChildren<objectSaver>();
		foreach (objectSaver childrenIDScript in childrenIds)
		{
			if (childrenIDScript.transform != go.transform)
			{
				if (string.IsNullOrEmpty(childrenIDScript.id) == true)
				{
					Destroy(childrenIDScript.gameObject);
				}
			}
		}
		return go;
	}
}
