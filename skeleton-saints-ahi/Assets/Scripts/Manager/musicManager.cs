using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class musicManager : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] List<AudioClip> musicTrackList;

    public AudioClip GetTrackWithName(string _trackName)
    {
        return musicTrackList.Find(x => x.name == _trackName);
    }

    public void PlayTrackWithName(string _trackName)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
