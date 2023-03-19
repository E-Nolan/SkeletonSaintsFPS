using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locationCondition : eventCondition
{
    public GameObject platformPrefab;
    public locationPlatform platformScript;

    public TaskListUI_Location ConditionUI;

    private void Start()
    {
        SpawnObjective();
    }
    public void SpawnObjective()
    {
        GameObject temp = Instantiate(platformPrefab, gameObject.transform);
        platformScript = temp.GetComponent<locationPlatform>();
        platformScript.Objective = temp.GetComponent<objectiveArea>();
        platformScript.Objective.parentCondition = this;

        
    }
    public override bool CheckCompletion()
    {
        if (platformScript.locationPathed)
        {
            satisfied = true;
        }
        else
        {
            satisfied = false;
        }
        return base.CheckCompletion();
    }
    public void UpdateLocationUI(locationCondition locate)
    {   if (locate.ConditionUI != null)
        {
            if (platformScript != null && gameManager.instance.PlayerScript() != null)
            {
                locate.ConditionUI.ConditionToggle.isOn = satisfied;
                locate.ConditionUI.ConditionalUIText.text = "Get to: " + locate.platformPrefab.name +
                    "\n Located at:" + platformScript.Objective.transform.position;
                locate.ConditionUI.LocationText.text = "Current Location: " +
                    gameManager.instance.PlayerScript().transform.position.ToString();
            }
        }
    }
}
