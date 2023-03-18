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

    public override string ToString()
    {
        string concatenation = "";
        foreach (gameObjectSaveData data in objectsData)
        {
            foreach (objectComponent oc in data.objectComponents)
                foreach (string field in oc.fields.Keys)
                {
                    concatenation += data.name + ":" + oc.componentName + "->" + field + ":" + oc.fields[field] + "\n";
                }
        }
        return concatenation;
    }
}

