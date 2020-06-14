using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_BackgroundMusicManager : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] AudioClips;

    private int currentClipIndex = -1;

    public void Play(int i, bool forcePlay = false)
    {
        if (i == -1)
        {
            GetComponent<AudioSource>().Stop();
            return;
        }
        
        if (
            i == currentClipIndex
            && !forcePlay
            && GetComponent<AudioSource>().isPlaying
        )
        {
            // continue track is isPlaying
            return;
        }

        GetComponent<AudioSource>().clip = AudioClips[i];
        GetComponent<AudioSource>().Play();

        currentClipIndex = i;
    }

    public void Stop()
    {
        GetComponent<AudioSource>().Stop();    
    }

    public void Pause()
    {
        GetComponent<AudioSource>().Pause();
    }

    public void UnPause()
    {
        GetComponent<AudioSource>().UnPause();
    }

    public bool GetIsPlaying()
    {
        return GetComponent<AudioSource>().isPlaying;
    }
}
