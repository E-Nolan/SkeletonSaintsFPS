using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReaction : DelayedReaction
{
    public Animator animator;
    public string text;

    protected override void ImmediateReaction()
    {
        if (instruction == 0)
        {
            animator.SetTrigger(text);
        }
        else if (instruction == 1)
        {
            animator.SetBool(text, !animator.GetBool(text));
        }
    }
}
