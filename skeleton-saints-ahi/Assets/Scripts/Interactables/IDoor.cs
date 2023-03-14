using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDoor : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
    }
    public void ActivateDoor()
    {
        if (anim.IsInTransition(0))
            return;
        AnimationReaction doorAction = ScriptableObject.CreateInstance<AnimationReaction>();
        doorAction.instruction = 1;
        doorAction.animator = anim;
        doorAction.text = "Activated";
        doorAction.React(this);
    }
}
