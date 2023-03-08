using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class interactableArea : MonoBehaviour
{
    [SerializeField] 
    MeshRenderer areaRenderer;

    [HideInInspector]
    public objectiveArea Objective;

    public bool Interacted;
    public bool PermanentlyOn;
    public Material InteractedMaterial;
    public Material OriginalMaterial;

    private void Awake()
    {
        if (areaRenderer != null)
            OriginalMaterial = areaRenderer.sharedMaterial;
    }
    public virtual void InteractWithArea()
    {
        if (PermanentlyOn)
            Interacted = true;
        else
            Interacted = !Interacted;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractWithArea();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InteractWithArea();
        }
    }
    public void ChangeColor()
    {
        if (Interacted)
        {
            areaRenderer.sharedMaterial = InteractedMaterial;
        }
        else
        {
            areaRenderer.sharedMaterial = OriginalMaterial;
        }
    }
}
