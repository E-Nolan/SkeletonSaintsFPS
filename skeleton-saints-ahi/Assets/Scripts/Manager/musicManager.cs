using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class musicManager : MonoBehaviour
{

    static musicManager instance;

    [SerializeField] List<AudioSource> audSources;
    [SerializeField] List<AudioClip> musicTrackList;
    [Range(0.0f, 10.0f)] [SerializeField] float fadeTimeSeconds;

    int currentAudIndex;
    List<bool> isFading = new List<bool>();

    private void Start()
    {
        instance = this;
        currentAudIndex = 0;

        for (int i = 0; i < audSources.Count; ++i)
        {
            isFading.Add(false);
        }
    }

    public AudioClip GetTrackWithName(string _trackName)
    {
        return musicTrackList.Find(x => x.name == _trackName);
    }

    public void StartTrackWithName(string _trackName)
    {
        if (audSources[currentAudIndex].clip.name != _trackName)
        {
            fadeOutTrack(audSources[currentAudIndex]);
            currentAudIndex = (audSources.Count - currentAudIndex) - 1;
        }
    }

    IEnumerator fadeOutTrack(AudioSource _fadingSource)
    {
        string _clipToFade = _fadingSource.clip.name;
        yield return new WaitForSeconds(fadeTimeSeconds);

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < audSources.Count; i++)
        {
            if (isFading[i])
            {
                audSources[i].volume -= (1.0f / fadeTimeSeconds) * Time.deltaTime;
                if (audSources[i].volume <= 0)
                {
                    audSources[i].clip = null;
                    isFading[i] = false;
                }
            }

        }
    }
}
