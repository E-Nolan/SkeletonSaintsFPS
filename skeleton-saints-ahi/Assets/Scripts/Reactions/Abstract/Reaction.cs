using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reaction : ScriptableObject
{
    /*Base reaction class that interactable objects will use when they provide specific reactions in terms of reacting with audio,
    providing information to the Game Manager's events, turning a behaviour on and off, causing an animation to occur, or sending
    information to the UI */
    public int instruction;
    //Init that calls the reaction specific Init
    public void Init()
    {
        SpecificInit();
    }
    protected virtual void SpecificInit()
    {}

    //Base reaction for the class to perform an immediate response, overwritten in classes that take time to react instead of an immediate
    //one
    public void React(MonoBehaviour component)
    {
        ImmediateReaction();
    }
    protected abstract void ImmediateReaction();
}
