using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_ThoughtManager : MonoBehaviour
{
    public CanvasGroup canvas;
    public AudioClip thoughtStartSoundFX;
    public AudioSource audioSource;
    public Text thoughtText;
    private IEnumerator coroutine;
    private bool isShowingThought;


    public float waitTimeBuffer;

    public void ShowThought(Model_Thought thought)
    {
        // stop any coroutine here
        if (isShowingThought)
        {
            StopCoroutine(coroutine);
            thoughtText.text = "";   
        }

        isShowingThought = true;
        
        thoughtText.text = thought.thought;

        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
        
        audioSource.PlayOneShot(thoughtStartSoundFX, 1.0f);
    }

    public void CloseThought(Model_Thought thoughtObj)
    {
        float waitTime = 0f;

        foreach(char letter in thoughtObj.thought.ToCharArray())
        {
            waitTime += 0.075f;
        }

        waitTime += waitTimeBuffer;

        print("waitimte: " + waitTime);
        coroutine = WaitToCloseThought(waitTime);

        StartCoroutine(coroutine);
    }

    IEnumerator WaitToCloseThought(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        HideThought();
        
        isShowingThought = false;
        thoughtText.text = "";
    }

    public void HideThought()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
    }

    public void Setup()
    {
        HideThought();    
    }
}
