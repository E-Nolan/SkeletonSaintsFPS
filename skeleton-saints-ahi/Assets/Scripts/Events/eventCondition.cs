using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class eventCondition : MonoBehaviour
{
    public gameEvent parentEvent;
    public int EventClass;

    public string description;

    [SerializeField]
    protected bool satisfied;

    public bool preRequisitesMet = false;
    private void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        if (parentEvent == null)
        {
            parentEvent = GetComponentInParent<gameEvent>();
        }
    }
    public bool Satisfied { get => satisfied;}

    //Updates the satisfied condition according to whatever type of event is implemented
    public virtual bool CheckCompletion()
    {
        return satisfied;
    }

    public virtual void ResetCondition()
    {
        satisfied = false;
    }
}
