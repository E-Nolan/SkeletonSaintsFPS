using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameEventManager : MonoBehaviour
{
    //A class with a static instance like gameManager to handle the GameEvents that are created and the conditions for each
    //public List<GameEvent> GameEvents;
    public int EventsCompleted;

    public static gameEventManager instance;

    public GameObject LocationEventText;
    public GameObject InteractionEventText;
    public GameObject CollectionEventText;
    public GameObject KillEventText;
    public GameObject EventTextGroup;
    public LayoutElement EventTextLayout;
    private void Awake()
    {
        instance = this;
    }
    public bool HasEvents()
    {
        //if (GameEvents != null && GameEvents.Count > 0)
        //{
        //    return true;
        //} else
        //{
        //    return false;
        //}
        return false;
    }
    public void FindEvents()
    {
        //GameObject[] objs = GameObject.FindGameObjectsWithTag("Game Event");
        //if (objs.Length > 0)
        //{
        //    foreach (GameObject obj in objs)
        //    {
        //        GameEvents.Add(obj.GetComponent<GameEvent>());
        //    }
        //}
    }

    public void GenerateEventsUI()
    {
        //if (GameEvents.Count > 0)
        //{
        //    foreach (GameEvent gameEvent in GameEvents)
        //    {
        //        GenerateEventUI(gameEvent);
        //    }
        //}
    }

    private void GenerateEventUI()
    {
        //if (gEvent.Conditions != null && gEvent.Conditions.Count != 0) {
        //    foreach (EventCondition eCondition in gEvent.Conditions)
        //    { 
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Location)
        //        {
        //            TaskListUI_Location locationUI = Instantiate(LocationEventText, EventTextGroup.transform).GetComponent<TaskListUI_Location>();
        //            EventTextLayout.preferredHeight += 50;
        //            locationUI.EventUIText.text = eCondition.description;
        //            (eCondition as LocationCondition).ConditionUI = locationUI;
        //            (eCondition as LocationCondition).UpdateLocationUI((LocationCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Interaction)
        //        {
        //            TaskListUI_Interaction interactionUI = Instantiate(InteractionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Interaction>();
                    
        //            interactionUI.EventUIText.text = eCondition.description;
        //            (eCondition as InteractionCondition).ConditionUI = interactionUI;
        //            (eCondition as InteractionCondition).UpdateInteractionUI((InteractionCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Collection)
        //        {
        //            TaskListUI_Collection collectionUI = Instantiate(CollectionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Collection>();
                    
        //            collectionUI.EventUIText.text = eCondition.description;
        //            (eCondition as CollectionCondition).ConditionUI = collectionUI;
        //            (eCondition as CollectionCondition).UpdateCollectionUI((CollectionCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Kill)
        //        {
        //            TaskListUI_Kill KillUI = Instantiate(CollectionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Kill>();

        //            KillUI.EventUIText.text = eCondition.description;
        //            (eCondition as KillCondition).ConditionUI = KillUI;
        //            (eCondition as KillCondition).UpdateKillUI((KillCondition)eCondition);
        //        }
        //    }
        //}
    }
    
    public void UpdateEvents()
    {
        //if (GameEvents.Count > 0)
        //{
        //    foreach (GameEvent gameEvent in GameEvents)
        //    {
        //        if (gameEvent.Conditions != null && gameEvent.Conditions.Count != 0)
        //        {
        //            UpdateEventUI(gameEvent);
        //        }
        //    }
        //}
    }
    private void UpdateEventUI()
    {
        //if (gEvent.Conditions.Count > 0)
        //{
        //    foreach (EventCondition eCondition in gEvent.Conditions)
        //    {
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Location)
        //        {
        //            (eCondition as LocationCondition).UpdateLocationUI((LocationCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Interaction)
        //        {
        //            (eCondition as InteractionCondition).UpdateInteractionUI((InteractionCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Collection)
        //        {
        //            (eCondition as CollectionCondition).UpdateCollectionUI((CollectionCondition)eCondition);
        //        }
        //        if (eCondition.EventClass == (int)ProjectUtilities.EventClass.Kill)
        //        {
        //            (eCondition as KillCondition).UpdateKillUI((KillCondition)eCondition);
        //        }
        //    }
        //}
    }

    public void ResetEvents()
    {
        ////If no events active just back out
        //if (GameEvents == null)
        //    return;
        //else
        //{
        //    //otherwise call the EventReset
        //    for (int i = 0; i < GameEvents.Count; i++)
        //    {
        //        GameEvents[i].EventReset();
        //    }
        //}
    }
    public static bool CheckEventCompletion()
    {
        ////Check if an Event's conditions are completed
        //gEvent.UpdateConditionsCompletion(gEvent.Conditions);
        //return gEvent.ReturnEventCompletion(gEvent.Conditions);
        return true;
    }
    public bool EventListComplete()
    {
        //List<GameEvent> allEvents = instance.GameEvents;

        //if (allEvents != null && allEvents[0] != null)
        //{
        //    for (int i = 0; i < allEvents.Count; i++)
        //    {
        //        if (!CheckEventCompletion(allEvents[i]))
        //        {
        //            return false;
        //        }
        //    }
        //}
        return true;
    }
    public void ClearEventListUI()
    {
        foreach(TaskListUIElement obj in EventTextGroup.GetComponentsInChildren<TaskListUIElement>())
        {
            Destroy(obj);
        }
    }
}
