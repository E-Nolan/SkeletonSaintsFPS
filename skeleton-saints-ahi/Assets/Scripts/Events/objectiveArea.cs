using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveArea : MonoBehaviour
{
    public locationCondition parentCondition;
    public interactableArea childArea;

    private void Awake()
    {
        childArea.Objective = this;
    }
    public void ObjectiveInteraction()
    {
        if (parentCondition != null)
            parentCondition.CheckCompletion();
    }
}
