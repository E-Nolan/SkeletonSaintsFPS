using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectReaction : DelayedReaction
{
    public GameObject gameObject;
    public bool activeState;

    protected override void ImmediateReaction()
    {
        gameObject.SetActive(activeState);
    }
}
