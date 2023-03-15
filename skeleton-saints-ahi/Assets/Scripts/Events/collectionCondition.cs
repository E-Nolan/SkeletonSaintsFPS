using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectionCondition : eventCondition
{
    public string CollectionName;

    //public List<gameEvent> Objectives;
    public List<GameObject> Objectives;
    public int FoundObjectives;

    //Locations in-level set for spawning of objectives
    [SerializeField]
    List<Transform> ObjectiveLocations;

    public TaskListUI_Collection ConditionUI;

    public bool keycardObjectives;

    private void Awake()
    {
        Init();
        
    }
    protected override void Init()
    {
        base.Init();
        SpawnCollectables();
    }
    public override bool CheckCompletion()
    {
        if (FoundObjectives != Objectives.Count)
        {
            satisfied = false;
            return base.CheckCompletion();
        }

        satisfied = true;
        return base.CheckCompletion();
    }
    public override void ResetCondition()
    {
        base.ResetCondition();
        FoundObjectives = 0;
    }

    public void SpawnCollectables()
    {
        if (!keycardObjectives)
        {
            for (int i = 0; i < Objectives.Count; i++)
            {
                itemPickup pickup = Instantiate(Objectives[i], ObjectiveLocations[i]).GetComponent<itemPickup>();
                pickup.parentCondition = this;
            }
        } else
        {
            for (int i = 0; i < Objectives.Count; i++)
            {
                IKey pickup = Instantiate(Objectives[i], ObjectiveLocations[i]).GetComponent<IKey>();
                pickup.parentCondition = this;
                pickup.AssignButton();
            }
        }
    }
    public void UpdateCollectionUI(collectionCondition collection)
    {
        if (collection.ConditionUI != null)
        {
            collection.ConditionUI.ConditionToggle.isOn = satisfied;
            collection.ConditionUI.ConditionalUIText.text = "Collect all of: " + CollectionName;
            collection.ConditionUI.collectiblesText.text = FoundObjectives.ToString() + "   /   " + Objectives.Count.ToString();
        }
    }

}
