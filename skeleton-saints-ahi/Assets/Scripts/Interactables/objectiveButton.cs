using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectiveButton : MonoBehaviour
{
    public interactionCondition parentCondition;
    public interactableButton childButton;

    private void Awake()
    {
        childButton.Objective = this;
    }
    public void ObjectiveInteraction()
    {
        if (parentCondition != null)
            parentCondition.CheckCompletion();
    }
}
