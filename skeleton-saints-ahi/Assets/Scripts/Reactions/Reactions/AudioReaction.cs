using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReaction : DelayedReaction
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    protected override void ImmediateReaction()
    {
        audioSource.clip = audioClip;
    }
}
