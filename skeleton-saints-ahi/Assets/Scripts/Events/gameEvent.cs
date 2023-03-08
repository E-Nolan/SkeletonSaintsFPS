using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEvent : MonoBehaviour
{
    //Title of event for UI description
    public string description;
    //Each event has a list of eventConditions like Collection, Location, Interaction
    public List<eventCondition> Conditions;

    public void EventReset()
    {
        //Reset event and all conditions to initial states
        if (Conditions == null)
            return;

        for (int i = 0; i < Conditions.Count; i++)
        {
            Conditions[i].ResetCondition();
        }
    }

    public void UpdateConditionsCompletion(List<eventCondition> conditions)
    {
        //Check if an Event's conditions are completed
        for (int i = 0; i < conditions.Count; i++)
        {
            conditions[i].CheckCompletion();
        }
    }
    public bool ReturnEventCompletion(List<eventCondition> conditions)
    {
        //Check if an Event's conditions are completed
        for (int i = 0; i < conditions.Count; i++)
        {
            if (!conditions[i].Satisfied)
            {
                return false;
            }
        }
        return true;
    }
}
