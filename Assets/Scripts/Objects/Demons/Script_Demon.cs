using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Demon : MonoBehaviour
{
    public Script_Game game;
    public AudioSource audioSource;
    public AudioClip deathCrySFX;
    

    private Animator animator;
    private Script_AudioOneShotSource audioOneShotSource;


    public int Id;
    public Model_Thought thought;


    private IEnumerator co;

    // Update is called once per frame
    void Update()
    {
        AdjustRotation();    
    }
    
    /* =============================================================        
        ANIMATION FUNCS BEGIN: called from animation
    ============================================================= */
    // called from: Demon_Default_swallowed-heart-ending
    private void FinishSwallowed()
    {
        /*
            if this swallowed animation was due to player eat action
            then check if player is eating and play eat-demon-heart animation
        */
        game.PlayerEatDemonHeart();

        game.EatDemon(Id);
        Destroy(this.gameObject);
    }
    /* =============================================================    
        END
    ============================================================= */

    public virtual void DefaultAction()
    {
        game.AddPlayerThought(thought);
        game.ShowAndCloseThought(thought);
        Swallowed();
    }

    private void Swallowed()
    {
        animator.SetTrigger("swallowedTrigger");
        audioOneShotSource = game.CreateAudioOneShotSource(transform.position);
        audioOneShotSource.Setup(deathCrySFX);
        audioOneShotSource.PlayOneShot();
    }


    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public virtual void Setup(
        Model_Thought _thought,
        AudioClip _deathCry
    )
    {
        game = FindObjectOfType<Script_Game>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        thought = _thought;
        deathCrySFX = _deathCry;

        AdjustRotation();
    }
}
