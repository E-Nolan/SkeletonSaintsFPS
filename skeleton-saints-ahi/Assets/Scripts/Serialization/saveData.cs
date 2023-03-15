using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class saveData
{
    public int Index = 0;
    public string name;
    public string Timestamp;

    public List<gameObjectSaveData> objectsData = new List<gameObjectSaveData>();
}

