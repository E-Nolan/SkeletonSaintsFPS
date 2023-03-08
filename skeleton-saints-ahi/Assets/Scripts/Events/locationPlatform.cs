using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locationPlatform : interactableArea
{
    public bool locationPathed;
    public override void InteractWithArea()
    {
        locationPathed = true;

        if (Objective != null)
        {
            Objective.ObjectiveInteraction();
        }
    }
}
