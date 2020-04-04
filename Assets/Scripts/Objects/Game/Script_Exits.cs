using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Exits : MonoBehaviour
{
    public AudioClip exitSFX;
    public CanvasGroup canvas;

    private AudioSource audioSource;
    private Script_Game game;
    private IEnumerator coroutine;


    public float InitiateLevelWaitTime;
    public float fadeSpeed;

    private bool exitsDisabled;
    private bool isFadeOut;
    private bool isFadeIn;
    private bool isHandlingExit;
    
    void Update()
    {
        if (isFadeOut)  FadeOut();
        if (isFadeIn)   FadeIn();    
    }

    public void HandleExit()
    {
        if (exitsDisabled || isHandlingExit)  return;

        if (!isHandlingExit) isHandlingExit = true;

        audioSource.PlayOneShot(exitSFX, 0.15f);

        // fade screen to black?

        // triggers fade out, initiatelevel called once fadeOutCompletes
        isFadeOut = true;
        game.ChangeStateToInitiateLevel();
        // game.InitiateLevel();
        
        // coroutine = WaitToInitiateLevel();
        // StartCoroutine(coroutine);
    }

    // IEnumerator WaitToInitiateLevel()
    // {
    //     yield return new WaitForSeconds(InitiateLevelWaitTime);

    //     game.InitiateLevel();
    // }

    public void DisableExits()
    {
        exitsDisabled = true;
    }

    public void EnableExits()
    {
        exitsDisabled = false;
    }

    public void StartFadeIn()
    {
        isFadeIn = true;
    }

    public void StartFadeOut()
    {
        isFadeOut = true;
    }

    void FadeOut()
    {
        canvas.alpha += fadeSpeed * Time.deltaTime;

        if (canvas.alpha >= 1f)
        {
            canvas.alpha = 1f;
            isFadeOut = false;
            
            game.DestroyLevel();
            
            isHandlingExit = false;
            
            // load next level
            game.level++;
            game.InitiateLevel();
            
            isFadeIn = true;
        }
    }

    void FadeIn()
    {
        canvas.alpha -= fadeSpeed * Time.deltaTime;

        if (canvas.alpha <= 0f)
        {
            canvas.alpha = 0f;
            isFadeIn = false;

            // after faded in, player can then move
            // change from initiate-level state
            game.SetInitialGameState();
        }
    }

    public void Setup(Script_Game _game)
    {
        game = _game;
        audioSource = GetComponent<AudioSource>();
    }
}
