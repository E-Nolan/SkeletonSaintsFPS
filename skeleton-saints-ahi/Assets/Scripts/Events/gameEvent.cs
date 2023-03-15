using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEvent : MonoBehaviour
{
    //Title of event for UI description
    public string description;
    //Each event has a list of eventConditions like Collection, Location, Interaction
    public List<eventCondition> Conditions;

    public List<eventCondition> SpecificConditions;

    public bool allPreConditionsMet;

    public void EventReset()
    {
        //Reset event and all conditions to initial states
        if (Conditions == null)
            return;
        allPreConditionsMet = false;
        for (int i = 0; i < Conditions.Count; i++)
        {
            Conditions[i].ResetCondition();
        }
        if (SpecificConditions == null)
            return;
        for (int i = 0; i < SpecificConditions.Count; i++)
        {
            SpecificConditions[i].ResetCondition();

        }
    }

    public void UpdateConditionsCompletion(List<eventCondition> conditions)
    {
        allPreConditionsMet = false;
        //Check if an Event's conditions are completed
        for (int i = 0; i < conditions.Count; i++)
        {
            conditions[i].CheckCompletion();
            allPreConditionsMet = conditions[i].CheckCompletion();
        }
        if (allPreConditionsMet)
        {
            for (int i = 0; i < SpecificConditions.Count; i++)
            {
                SpecificConditions[i].preRequisitesMet = true;
                SpecificConditions[i].CheckCompletion();
            }
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
        for (int i = 0; i < SpecificConditions.Count; i++)
        {
            if (!SpecificConditions[i].Satisfied)
            {
                return false;
            }
        }
        return true;
    }
    public void SetParent()
    {
        for (int i = 0; i < Conditions.Count; i++)
        {
            Conditions[i].parentEvent = this;
        }
        for (int i = 0; i < SpecificConditions.Count; i++)
        {
            SpecificConditions[i].parentEvent = this;
        }
    }
}
