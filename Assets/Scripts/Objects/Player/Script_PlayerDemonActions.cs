using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerDemonActions : MonoBehaviour
{
    public float eatingTime;


    private Script_Game game;
    private Animator animator;
    public AudioClip crunchSFX;
    private Script_AudioOneShotSource audioOneShotSource;


    private Script_Player player;
    private IEnumerator co;

/* =============================================================        
        ANIMATION FUNCS BEGIN: called from animation
    ============================================================= */
    // called from demon animation > game > here: Demon_Default-swallowed-heart-ending 
    public void EatHeart()
    {
        // swallowing heart animation on trigger
        animator.SetTrigger("Eat");

        // animation exits back to Player_eat-demon automatically
    }

    // called from: Player_eat-demon-heart-[direction] animation func
    private void EndEating()
    {
        animator.SetBool("IsEating", false);
        player.SetIsNotEating();
    }
    /* =============================================================    
        END
    ============================================================= */

    public void EatDemon()
    {
        // play eating animation
        player.SetIsEating();
        OpenMouth();
    }

    private void OpenMouth()
    {
        // this conditional transition in animator causes mouth open
        animator.SetBool("IsEating", true);
        
        audioOneShotSource = game.CreateAudioOneShotSource(transform.position);
        audioOneShotSource.Setup(crunchSFX);
        audioOneShotSource.PlayOneShot();
    }


    public void Setup(Script_Game _game)
    {
        game = _game;
        player = GetComponent<Script_Player>();
        animator = player.animator;
    }
}
