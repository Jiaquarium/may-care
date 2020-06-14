using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Glimmer : MonoBehaviour
{
    public Animator a;
    public AudioSource audioSource;
    public AudioClip glimmerSFX;
    public float glimmerSFXVolScale;
    
    public void Glimmer()
    {
        a.SetTrigger("glimmer");
        audioSource.PlayOneShot(glimmerSFX, glimmerSFXVolScale);
    }
}
