using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_AudioOneShotSource : MonoBehaviour
{   
    private AudioClip clip;
    private AudioSource src;
    private bool startedPlaying;
    
    void Update()
    {
        if (startedPlaying && !src.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
    
    public void PlayOneShot(float v = 1.0f)
    {
        src.PlayOneShot(clip, v);
        startedPlaying = true;
    }

    public void Setup(AudioClip _clip)
    {
        src = GetComponent<AudioSource>();
        clip = _clip;
    }
}
