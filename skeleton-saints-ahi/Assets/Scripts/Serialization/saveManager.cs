using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
public static class saveManager
{
    public static saveData mainData_Current = new saveData();
    public static saveData checkpointData_Current = new saveData();

    public static saveManagerInterface saveHelper;
    public static int saveNumberMain = 0;

    public static int saveNumberCheckpoint = 0;
    public const string mainDataPath = "/SaveData/Main/";

    public static string directory = Application.persistentDataPath + mainDataPath;

    public const string extension = ".sav";


    public delegate void SaveData(saveData Data);

    public static event SaveData SaveDataEvent;

    public static int GetItemsSaved
    {
        get
        {
            return SaveDataEvent.GetInvocationList().Length;
        }
    }
    public static void TriggerSerialization(saveData data)
    {
        if (SaveDataEvent != null)
            SaveDataEvent.Invoke(data);
    }
    public static void playerSave()
    {
        gameObjectSaveData newPlayerData = new gameObjectSaveData(gameManager.instance.playerInstance);
        objectSaver saver = gameManager.instance.playerInstance.GetComponent<objectSaver>();
        saver.SetID();
        gameManager.instance.playerInstance.SendMessage("OnSerialize", SendMessageOptions.DontRequireReceiver);
        TriggerSerialization(mainData_Current);
    }
    public static bool SaveGameData(saveData save)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);

        }

        string json;
        string fullpath;
        save.Timestamp = DateTime.Now.ToString();
        //if the path is the main one, use main data. if not, it is checkpoint
        fullpath = directory + save.name + extension;

        if (!File.Exists(fullpath))
        {
            File.Delete(fullpath);
        }

        mainData_Current = save;

        json = JsonUtility.ToJson(mainData_Current, prettyPrint: true);
        File.WriteAllText(path: fullpath, contents: json);
        saveNumberMain++;
        GUIUtility.systemCopyBuffer = directory;
        return true;
    }
    public static saveData LoadGameData(string path)
    {
        string fullPath = path + extension;

        saveData tempData = new saveData();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            tempData = JsonUtility.FromJson<saveData>(json);
        }
        else
        {
            Debug.LogError(message: "Save File doesn't exist");
            Debug.LogError(message: fullPath);
        }

        mainData_Current = tempData;

        return mainData_Current;
    }
}


