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
        if (Objective != null)
        {
            Objective.ObjectiveInteraction();
        }
        if (CanInteractYet)
        {
            if (PermanentlyOn && Interacted == true)
                return;
            base.Interact();
            LinkedDoor.ActivateDoor();
            if (!onDoor)
                ChangeColor();
        }

    }
}
