using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelayedReaction : Reaction
{
    //A reaction class that allows for a timed delay before execution. This is most useful in cases where a frame or two may be needed
    //for the effect to process
    public float delay;

    protected WaitForSeconds wait;

    //A change to the base Init to assign a delay to the wait and then call the spefic one
    public new void Init()
    {
        wait = new WaitForSeconds(delay);

        SpecificInit();
    }

    //Overwrite to the base react which calls for an immediate one.
    //This starts the DelayedReaction's ReactCoroutine()
    public new void React(MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(ReactCoroutine());
    }

    protected IEnumerator ReactCoroutine()
    {
        yield return wait;
        ImmediateReaction();
    }
}
