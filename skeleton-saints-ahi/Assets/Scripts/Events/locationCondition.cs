using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locationCondition : eventCondition
{
    public GameObject platformPrefab;
    public locationPlatform Objective;

    public TaskListUI_Location ConditionUI;

    private void Start()
    {
        SpawnObjective();
    }
    public void SpawnObjective()
    {
        Instantiate(platformPrefab, gameObject.transform);
        Objective = platformPrefab.GetComponent<locationPlatform>();
    }
    public override bool CheckCompletion()
    {
        if (Objective.locationPathed)
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
            locate.ConditionUI.ConditionToggle.isOn = satisfied;
            locate.ConditionUI.ConditionalUIText.text = "Get to: " + locate.platformPrefab.name + 
                "\n Located at:" + Objective.transform.position;
            locate.ConditionUI.LocationText.text = "Current Location: " +
                gameManager.instance.PlayerScript().transform.position.ToString();
        }
    }
}
