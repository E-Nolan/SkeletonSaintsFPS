using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorButton : interactableButton, IInteractable
{
    [SerializeField]
    private IDoor LinkedDoor;
    public bool onDoor;

    public override void Interact()
    {
        base.Interact();
        LinkedDoor.ActivateDoor();
        if (!onDoor)
            ChangeColor();
        if (Objective != null)
        {
            Objective.ObjectiveInteraction();
        }
    }
}
