using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactableButton : IInteractable
{

    [SerializeField]
    MeshRenderer ButtonRenderer;

    [HideInInspector]
    public objectiveButton Objective;

    public bool CanInteractYet = false;

    public bool Interacted;
    public bool InteractedOnce;
    public bool PermanentlyOn;
    public bool ChangesColor;
    public bool toggleable;

    public Material InteractedMaterial;
    public Material OriginalMaterial;

    private void Start()
    {
        if (Objective == null)
        {
            CanInteractYet = true;
        }
        if (ChangesColor && toggleable)
            OriginalMaterial = ButtonRenderer.sharedMaterial;
    }
    public override void Interact()
    {
        InteractedOnce = true;

        if (PermanentlyOn && Interacted == true)
            return;
        else
        {
            Interacted = !Interacted;
        }
    }

    public void ChangeColor(bool colorON = true)
    {
        if (!ChangesColor)
            return;

        ButtonRenderer.sharedMaterial = colorON ? InteractedMaterial : OriginalMaterial;
    }
}
