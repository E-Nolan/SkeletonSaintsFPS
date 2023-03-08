using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField]
    MeshRenderer ButtonRenderer;

    [HideInInspector]

    public bool Interacted;
    public bool InteractedOnce;
    public bool PermanentlyOn;
    public bool ChangesColor;
    public bool toggleable;

    public Material InteractedMaterial;
    public Material OriginalMaterial;

    private void Awake()
    {
        if (ChangesColor && toggleable)
            OriginalMaterial = ButtonRenderer.sharedMaterial;
    }
    public virtual void Interact()
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
