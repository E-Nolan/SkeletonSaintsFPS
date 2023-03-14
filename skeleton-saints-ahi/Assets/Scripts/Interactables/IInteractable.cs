using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IInteractable: MonoBehaviour
{
    public string originalInteractionText = "Interact";
    public string currentInteractionText = "Interact";

    public void ResetInteractionText()
    {
        currentInteractionText = originalInteractionText;
    }
    public virtual void Interact()
    {

    }
}
