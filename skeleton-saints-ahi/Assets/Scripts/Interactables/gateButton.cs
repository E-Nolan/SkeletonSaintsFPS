using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateButton : interactableButton
{
    [SerializeField]
    protected IGate LinkedGate;

    [Tooltip("1 = Red, 2 = Yellow, 3 =  Blue\n0 means no card is required")]
    public int accessLevel = 0;
    
    protected string accessGranted = "ACCESS GRANTED";
    string accessDeniedRed = "ACCESS DENIED:\nLevel 1: Red Clearance required";
    string accessDeniedBlue = "ACCESS DENIED:\nLevel 2: Blue Clearance required";
    string accessDeniedYellow = "ACCESS DENIED:\nLevel 3: Yellow Clearance required";

    protected string accessDeniedFull = "ACCESS DENIED:\nFull Clearance required";

    //For when the door can't be interacted with but doesn't need kaycard clearance (e.g locked from somewhere else)
    string accessDeniedOpenClearance = "ACCESS DENIED:\nGate is deadlocked.";

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
            LinkedGate.isUnlocked = true;
            LinkedGate.ActivateGate();
            currentInteractionText = accessGranted;
            ChangeColor();
        }
        else
        {
            switch (accessLevel)
            {
                case 0:
                    currentInteractionText = accessDeniedOpenClearance;
                    break;
                case 1:
                    currentInteractionText = accessDeniedRed;
                    break;
                case 2:
                    currentInteractionText = accessDeniedBlue;
                    break;
                case 3:
                    currentInteractionText = accessDeniedYellow;
                    break;
                case 4:
                    currentInteractionText = accessDeniedFull;
                    break;
                default:
                    currentInteractionText = accessDeniedOpenClearance;
                    break;
            }
        }
    }
}
