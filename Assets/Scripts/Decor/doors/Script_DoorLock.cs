using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DoorLock : MonoBehaviour
{
    public int id;
    public float unlockSFXVolScale;
    [SerializeField]
    private Animator a;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip unlockClip;
    private Script_Game game;
    
    void Awake()
    {
        game = FindObjectOfType<Script_Game>();
        a = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        unlockClip = audioSource.clip;
    }

    public void Unlock()
    {
        a.SetTrigger("unlock");
        audioSource.PlayOneShot(unlockClip, unlockSFXVolScale);
    }

    public void UnlockCallback()
    {
        game.OnDoorLockUnlock(id);
    }
}
