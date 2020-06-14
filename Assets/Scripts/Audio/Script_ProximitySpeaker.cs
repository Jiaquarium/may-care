using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    drop this gameObject into the World
*/
public class Script_ProximitySpeaker : MonoBehaviour
{
    protected Script_Game game;
    public AudioSource audioSource;
    public float maxDistance;

    protected virtual void OnDisable()
    {
        audioSource.Stop();
    }

    protected virtual void OnEnable()
    {
        AdjustVolume();
        audioSource.Play();
    }
    
    protected virtual void Update()
    {
        AdjustVolume();
    }

    protected void AdjustVolume()
    {
        if (!game.GetPlayerIsSpawned())    return;
        
        float distance = Vector3.Distance(game.GetPlayerLocation(), transform.position);
        if (distance >= maxDistance)
        {
            audioSource.volume = 0f;
        }
        else
        {
            float v = distance / maxDistance;
            audioSource.volume = 1f - v;
        }
    }

    protected virtual void Awake()
    {
        game = Object.FindObjectOfType<Script_Game>();
        GetComponent<SpriteRenderer>().enabled = false;

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
