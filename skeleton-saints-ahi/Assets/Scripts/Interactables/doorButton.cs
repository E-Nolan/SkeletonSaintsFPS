using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorButton : interactableButton
{
    [SerializeField]
    private IDoor LinkedDoor;
    public bool onDoor;

    public bool secretDoor;

    string activateSecretDoor = "ACCESS GRANTED";
    string activateSecretDoorDenial = "ACCESS DENIED";
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
            if (secretDoor)
                interactionText = activateSecretDoor;

            if (!onDoor)
                ChangeColor();
        }
        else
        {
            if (secretDoor)
                interactionText = activateSecretDoorDenial;
        }

    }
}
