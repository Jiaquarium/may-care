using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Script_DemonStats))]
public class Script_Demon : Script_Character
{
    public int Id;
    public Model_Thought thought;
    public int swallowedFillCount;
    public AudioClip deathCrySFX;
    

    private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer spriteRenderer;
    private Script_AudioOneShotSource audioOneShotSource;
    

    private IEnumerator co;
    
    [SerializeField]
    private bool isInvincible;


    // Update is called once per frame
    void Update()
    {
        AdjustRotation();    
    }
    
    /// <summary>
    /// start demon dying sequence
    /// </summary>
    public void Die()
    {
        if (isInvincible)   return;
        isInvincible = true;

        Script_Game.Game.AddPlayerThought(thought);
        Script_Game.Game.ShowAndCloseThought(thought);

        Script_Game.Game.FillHearts(swallowedFillCount);

        Swallowed();
    }
    
    public void FinishSwallowed()
    {
        isInvincible = false;
        Script_Game.Game.EatDemon(Id);
        gameObject.SetActive(false);
    }
    

    private void Swallowed()
    {
        animator.SetTrigger("swallowedTrigger");
        audioOneShotSource = Script_Game.Game.CreateAudioOneShotSource(transform.position);
        audioOneShotSource.Setup(deathCrySFX);
        audioOneShotSource.PlayOneShot();
    }


    public void AdjustRotation()
    {
        spriteRenderer.transform.forward = Camera.main.transform.forward;
    }

    public virtual void Setup(
        Model_Thought _thought,
        AudioClip _deathCry
    )
    {
        audioSource = GetComponent<AudioSource>();
        
        thought = _thought;
        deathCrySFX = _deathCry;

        /// Setup character stats
        base.Setup();

        AdjustRotation();
    }
}
