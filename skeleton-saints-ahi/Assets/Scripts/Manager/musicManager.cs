using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class musicManager : MonoBehaviour
{

    public static musicManager instance;

    [SerializeField] List<AudioSource> audSources;
    [SerializeField] List<AudioClip> musicTrackList;
    [Range(0.0f, 10.0f)] [SerializeField] float fadeTimeSeconds;

    int currentAudIndex;
    public List<bool> isFading = new List<bool>();

    private void Start()
    {
        instance = this;
        currentAudIndex = 0;

        for (int i = 0; i < audSources.Count; ++i)
        {
            isFading.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < audSources.Count; i++)
        {
            if (isFading[i])
            {
                if (i == currentAudIndex)
                {
                    if (1.0f == (audSources[i].volume = Mathf.Clamp(audSources[i].volume + (1.0f / fadeTimeSeconds) * Time.deltaTime, 0.0f, 1.0f)))
                    {
                        isFading[i] = false;
                    }
                }
                else
                {
                    if (0.0f == (audSources[i].volume = Mathf.Clamp(audSources[i].volume - (1.0f / fadeTimeSeconds) * Time.deltaTime, 0.0f, 1.0f)))
                    {
                        isFading[i] = false;
                        audSources[i].Stop();
                        audSources[i].clip = null;
                    }
                }
            }
        }
    }

    public void AddMusicTrackToList(AudioClip _newTrack)
    {
        if (musicTrackList.Find(x => x.name == _newTrack.name) == null)
        {
            //Debug.Log($"Adding {_newTrack.name} to the playlist");
            musicTrackList.Add(_newTrack);
        }
    }

    public AudioClip GetTrackWithName(string _trackName)
    {
        return musicTrackList.Find(x => x.name == _trackName);
    }


    public void StartMusicTrack(AudioClip _musicTrack)
    {
        AddMusicTrackToList(_musicTrack);
        StartTrackWithName(_musicTrack.name);
    }

    public void StartTrackWithName(string _trackName)
    {
        if (audSources[currentAudIndex].clip != GetTrackWithName(_trackName))
        {
            fadeOutTrack(currentAudIndex);

            currentAudIndex = (audSources.Count - currentAudIndex) - 1;

            setClipInAudioSource(_trackName, currentAudIndex);
            //audSources[currentAudIndex].clip = GetTrackWithName(_trackName);
            if (audSources[currentAudIndex].clip != null)
                audSources[currentAudIndex].Play();
            audSources[currentAudIndex].volume = 0.0f;
            fadeInTrack(currentAudIndex);
        }
    }

    void fadeOutTrack(int _audIndex)
    {
        isFading[_audIndex] = true;
    }

    void fadeInTrack(int _audIndex)
    {
        isFading[_audIndex] = true;
    }

    void setClipInAudioSource(string _trackName, int _audIndex)
    {
        AudioClip _clip = GetTrackWithName(_trackName);
        audSources[_audIndex].clip = _clip;
        //Debug.Log($"Playing {audSources[_audIndex].clip.name}");
    }
}
