using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorButton : interactableButton
{
    [SerializeField]
    private IDoor LinkedDoor;
    public bool onDoor;

    public bool secretDoor;

    string activateSecretDoor = "ACCESS GRANTED: Welcome, Dr.Rikayon";
    string activateSecretDoorDenial = "ACCESS DENIED: Personal Code Entry Incorrect";

    private void Awake()
    {
        
    }
    public override void Init()
    {
        base.Init();
        //set to false specifically for gatebutton since by default interactables start off with this true
        CanInteractYet = false;
    }
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
                currentInteractionText = activateSecretDoor;

            if (!onDoor)
                ChangeColor();
        }
        else
        {
            if (secretDoor)
                currentInteractionText = activateSecretDoorDenial;
        }

    }
}
