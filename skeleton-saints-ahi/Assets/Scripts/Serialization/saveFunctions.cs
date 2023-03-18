using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class saveFunctions : MonoBehaviour
{
    public void SaveMain()
    {
        saveManager.mainData_Current.Index++;
        saveManager.saveHelper.SaveGame("Main Save");
    }
    //if 0 it will load the most recent save.
    public void LoadMain()
    {
        saveManager.saveHelper.LoadGame("Main Save");

    }
}
