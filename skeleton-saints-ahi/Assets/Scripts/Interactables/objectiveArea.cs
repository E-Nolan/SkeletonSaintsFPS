using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveArea : MonoBehaviour
{
    public eventCondition parentCondition;
    public interactableArea childArea;

    private void Awake()
    {
        childArea.Objective = this;
    }
    public virtual void ObjectiveInteraction()
    {
        if (parentCondition != null)
            parentCondition.CheckCompletion();
    }
}
