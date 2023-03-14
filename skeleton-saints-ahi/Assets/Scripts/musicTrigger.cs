using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip musicTrack;
    [Range(0.0f, 10.0f)] [SerializeField] float triggerCooldown;

    bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            StartCoroutine(startPlayingMusic());
        }
    }

    IEnumerator startPlayingMusic()
    {
        hasTriggered = true;
        musicManager.instance.StartMusicTrack(musicTrack);
        yield return new WaitForSeconds(triggerCooldown);
        hasTriggered = false;
    }
}
