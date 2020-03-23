using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BackgroundMusicManager : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] AudioClips;

    private int currentClipIndex = -1;

    public void Play(int i)
    {
        if (i == currentClipIndex)  return;

        GetComponent<AudioSource>().clip = AudioClips[i];
        GetComponent<AudioSource>().Play();

        currentClipIndex = i;
    }

    public void Pause()
    {
        GetComponent<AudioSource>().Pause();
    }

    public void UnPause()
    {
        GetComponent<AudioSource>().UnPause();
    }
}
