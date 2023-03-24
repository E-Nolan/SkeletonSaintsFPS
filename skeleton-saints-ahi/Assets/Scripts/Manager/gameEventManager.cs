using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameEventManager : MonoBehaviour
{
    //A class with a static instance like gameManager to handle the gameEvents that are created and the conditions for each
    public List<gameEvent> gameEvents;
    public int EventsCompleted;

    public static gameEventManager instance;
    
    public GameObject LocationEventText;
    public GameObject InteractionEventText;
    public GameObject CollectionEventText;
    public GameObject KillEventText;

    [Header("References")]
    public GameObject EventTextGroup;
    public List<TaskListUIElement> EventTexts;
    public LayoutElement EventTextLayout;
    public GameObject EventTextBody;
    public GameObject MainEventBody;



    private void Awake()
    {
        instance = this;
        EventTexts = new List<TaskListUIElement>();
    }
    public bool HasEvents()
    {
        if (gameEvents != null && gameEvents.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void FindEvents()
    {
        gameEvents.Clear();
        ClearEventListUI();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameEvent");
        if (objs.Length > 0)
        {
            foreach (GameObject obj in objs)
            {
                gameEvents.Add(obj.GetComponent<gameEvent>());
            }
        }
        GenerateEventsUI();
    }
    public bool HasKillCondition(out killCondition foundCondition)
    {
        bool found = false;
        if (gameEvents.Count > 0)
        {
            for(int i = 0; i < gameEvents.Count; i++)
            {
                if (gameEvents[i].Conditions != null)
                {
                    for (int j = 0; j < gameEvents[i].Conditions.Count; j++)
                    {
                        if (gameEvents[i].Conditions[j].EventClass == 3)
                        {
                            if ((gameEvents[i].Conditions[j] as killCondition).enemiesLeft != 0)
                            {
                                foundCondition = (gameEvents[i].Conditions[j] as killCondition);
                                found = true;
                                return found;
                            }
                        }
                    }
                }
                if (gameEvents[i].SpecificConditions != null)
                {
                    for (int k = 0; k < gameEvents[i].SpecificConditions.Count; k++)
                    {
                        if (gameEvents[i].SpecificConditions[k].EventClass == 3)
                        {
                            if ((gameEvents[i].SpecificConditions[k] as killCondition).enemiesLeft != 0)
                            {
                                foundCondition = (gameEvents[i].SpecificConditions[k] as killCondition);
                                found = true;
                                return found;
                            }
                        }
                    }
                }
            }
        }
        foundCondition = null;
        return found;
    }
    public void GenerateEventsUI()
    {
        if (gameEvents.Count > 0)
        {
            foreach (gameEvent gameEvent in gameEvents)
            {
                GenerateEventUI(gameEvent);
            }
        }
    }

    private void GenerateEventUI(gameEvent gEvent)
    {
        if (gEvent.Conditions != null && gEvent.Conditions.Count != 0)
        {
            foreach (eventCondition eCondition in gEvent.Conditions)
            {
                if (eCondition.EventClass == (int)gameManager.EventClass.Location)
                {
                    TaskListUI_Location locationUI = Instantiate(LocationEventText, EventTextGroup.transform).GetComponent<TaskListUI_Location>();
                    EventTexts.Add(locationUI);
                    modifyEventText(50, 100);
                    locationUI.EventUIText.text = eCondition.description;
                    (eCondition as locationCondition).ConditionUI = locationUI;
                    (eCondition as locationCondition).UpdateLocationUI((locationCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Interaction)
                {
                    TaskListUI_Interaction interactionUI = Instantiate(InteractionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Interaction>();
                    EventTexts.Add(interactionUI);
                    modifyEventText(50, 100);
                    interactionUI.EventUIText.text = eCondition.description;
                    (eCondition as interactionCondition).ConditionUI = interactionUI;
                    (eCondition as interactionCondition).UpdateInteractionUI((interactionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Collection)
                {
                    if ((eCondition as collectionCondition).keycardObjectives)
                    {
                        TaskListUI_Collection collectionUI = Instantiate(CollectionEventText, MainEventBody.transform).GetComponent<TaskListUI_Collection>();
                        EventTexts.Add(collectionUI);
                        modifyEventText(50, 100);
                        collectionUI.EventUIText.text = eCondition.description;
                        (eCondition as collectionCondition).ConditionUI = collectionUI;
                        (eCondition as collectionCondition).UpdateCollectionUI((collectionCondition)eCondition);
                    } else
                    {
                        TaskListUI_Collection collectionUI = Instantiate(CollectionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Collection>();
                        EventTexts.Add(collectionUI);
                        modifyEventText(50, 100);
                        collectionUI.EventUIText.text = eCondition.description;
                        (eCondition as collectionCondition).ConditionUI = collectionUI;
                        (eCondition as collectionCondition).UpdateCollectionUI((collectionCondition)eCondition);
                    }

                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Kill)
                {
                    TaskListUI_Kill KillUI = Instantiate(KillEventText, EventTextGroup.transform).GetComponent<TaskListUI_Kill>();
                    EventTexts.Add(KillUI);
                    modifyEventText(50, 100);
                    KillUI.EventUIText.text = eCondition.description;
                    (eCondition as killCondition).ConditionUI = KillUI;
                    (eCondition as killCondition).UpdateKillUI((killCondition)eCondition);
                }
            }
        }
        if (gEvent.SpecificConditions != null && gEvent.Conditions.Count != 0)
        {
            foreach (eventCondition eCondition in gEvent.SpecificConditions)
            {
                if (eCondition.EventClass == (int)gameManager.EventClass.Location)
                {
                    TaskListUI_Location locationUI = Instantiate(LocationEventText, EventTextGroup.transform).GetComponent<TaskListUI_Location>();
                    EventTexts.Add(locationUI);
                    modifyEventText(50, 100);
                    locationUI.EventUIText.text = eCondition.description;
                    (eCondition as locationCondition).ConditionUI = locationUI;
                    (eCondition as locationCondition).UpdateLocationUI((locationCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Interaction)
                {
                    TaskListUI_Interaction interactionUI = Instantiate(InteractionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Interaction>();
                    modifyEventText(50, 100);
                    EventTexts.Add(interactionUI);
                    interactionUI.EventUIText.text = eCondition.description;
                    (eCondition as interactionCondition).ConditionUI = interactionUI;
                    (eCondition as interactionCondition).UpdateInteractionUI((interactionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Collection)
                {
                    TaskListUI_Collection collectionUI = Instantiate(CollectionEventText, EventTextGroup.transform).GetComponent<TaskListUI_Collection>();
                    modifyEventText(50, 100);
                    EventTexts.Add(collectionUI);
                    collectionUI.EventUIText.text = eCondition.description;
                    (eCondition as collectionCondition).ConditionUI = collectionUI;
                    (eCondition as collectionCondition).UpdateCollectionUI((collectionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Kill)
                {
                    TaskListUI_Kill KillUI = Instantiate(KillEventText, EventTextGroup.transform).GetComponent<TaskListUI_Kill>();
                    modifyEventText(50, 100);
                    EventTexts.Add(KillUI);
                    KillUI.EventUIText.text = eCondition.description;
                    (eCondition as killCondition).ConditionUI = KillUI;
                    (eCondition as killCondition).UpdateKillUI((killCondition)eCondition);
                }
            }
        }
    }
    
    public void UpdateEvents()
    {
        if (gameEvents.Count > 0)
        {
            foreach (gameEvent gameEvent in gameEvents)
            {
                if (gameEvent.Conditions != null && gameEvent.Conditions.Count != 0)
                {
                    UpdateEventUI(gameEvent);
                }
            }
        }
    }
    private void UpdateEventUI(gameEvent gEvent)
    {
        if (gEvent.Conditions.Count > 0)
        {
            foreach (eventCondition eCondition in gEvent.Conditions)
            {
                if (eCondition.EventClass == (int)gameManager.EventClass.Location)
                {
                    (eCondition as locationCondition).UpdateLocationUI((locationCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Interaction)
                {
                    (eCondition as interactionCondition).UpdateInteractionUI((interactionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Collection)
                {
                    (eCondition as collectionCondition).UpdateCollectionUI((collectionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Kill)
                {
                    (eCondition as killCondition).UpdateKillUI((killCondition)eCondition);
                }
            }
        }
        if (gEvent.SpecificConditions.Count > 0)
        {
            foreach (eventCondition eCondition in gEvent.SpecificConditions)
            {
                if (eCondition.EventClass == (int)gameManager.EventClass.Location)
                {
                    (eCondition as locationCondition).UpdateLocationUI((locationCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Interaction)
                {
                    (eCondition as interactionCondition).UpdateInteractionUI((interactionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Collection)
                {
                    (eCondition as collectionCondition).UpdateCollectionUI((collectionCondition)eCondition);
                }
                if (eCondition.EventClass == (int)gameManager.EventClass.Kill)
                {
                    (eCondition as killCondition).UpdateKillUI((killCondition)eCondition);
                }
            }
        }
    }
    void modifyEventText(int layoutHeight, int bodyPosition)
    {
        EventTextLayout.preferredHeight += layoutHeight;
        RectTransform currentRect = EventTextBody.GetComponent<RectTransform>();
        EventTextBody.GetComponent<RectTransform>().sizeDelta = new Vector2(500, currentRect.sizeDelta.y + bodyPosition);
        Vector3 newPos = new Vector3(currentRect.position.x, currentRect.position.y - layoutHeight, currentRect.position.z);
        EventTextBody.GetComponent<RectTransform>().SetPositionAndRotation(newPos, currentRect.rotation);
    }
    public void ResetEvents()
    {
        //If no events active just back out
        if (gameEvents == null)
            return;
        else
        {
            //otherwise call the EventReset
            for (int i = 0; i < gameEvents.Count; i++)
            {
                gameEvents[i].EventReset();
            }
        }
    }
    public static bool CheckEventCompletion(gameEvent gEvent)
    {
        ////Check if an Event's conditions are completed
        gEvent.UpdateConditionsCompletion(gEvent.Conditions);
        return gEvent.ReturnEventCompletion(gEvent.Conditions);
    }
    public bool EventListComplete()
    {
        List<gameEvent> allEvents = instance.gameEvents;

        if (allEvents != null && allEvents[0] != null)
        {
            for (int i = 0; i < allEvents.Count; i++)
            {
                if (!CheckEventCompletion(allEvents[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void ClearEventListUI()
    {
        if (EventTexts.Count != 0)
        {
            foreach (TaskListUIElement obj in EventTextGroup.GetComponentsInChildren<TaskListUIElement>())
            {
                Destroy(obj.gameObject);
                modifyEventText(-50, -100);
            }
            foreach (TaskListUIElement obj in MainEventBody.GetComponentsInChildren<TaskListUIElement>())
            {
                Destroy(obj.gameObject);
            }
            EventTexts.Clear();
           
        }
    }
}
