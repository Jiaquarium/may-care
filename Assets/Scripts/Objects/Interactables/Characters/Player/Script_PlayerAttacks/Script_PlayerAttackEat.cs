using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// hits only 1 enemy at a time
/// return on CollisionedWith after 1 hit and hitbox is hidden
/// </summary>
public class Script_PlayerAttackEat : Script_Attack
{
    [SerializeField] private float eatingTime;
    [SerializeField] private AudioClip crunchSFX;
    [SerializeField] private Script_Player player;
    [SerializeField] private Animator animator;

    private bool didHit;
    private Coroutine eatingCoroutine;
    private Script_Game game;
    private Script_AudioOneShotSource audioOneShotSource;
    private IEnumerator co;

    /* =============================================================        
        ANIMATION FUNCS BEGIN: called from animation
    ============================================================= */
    // called from demon animation > game > here: Demon_Default-swallowed-heart-ending 
    // CURRENTLY NOT USING
    public void EatHeart()
    {
        // swallowing heart animation on trigger
        animator.SetTrigger("Eat");
    }

    /* =============================================================    
        END
    ============================================================= */

    public void Eat(string direction)
    {
        // disallow any more action inputs
        player.SetIsAttacking();
        
        // play eating animation
        OpenMouth();

        didHit = false;
        base.Attack(direction);
        eatingCoroutine = StartCoroutine(EndEating());
    }

    IEnumerator EndEating()
    {
        yield return new WaitForSeconds(eatingTime);

        base.EndAttack();
        
        animator.SetBool("IsEating", false);
        player.SetLastState();
    }

    private void OpenMouth()
    {
        // this conditional transition in animator causes mouth open
        animator.SetBool("IsEating", true);
        
        audioOneShotSource = Script_Game.Game.CreateAudioOneShotSource(transform.position);
        audioOneShotSource.Setup(crunchSFX);
        audioOneShotSource.PlayOneShot();
    }

    public override void CollisionedWith(Collider collider)
    {
        // only allow one hit for Eat
        if (didHit)
        { 
            activeHitBox.StopCheckingCollision();
            return;
        }
        Script_HurtBox hurtBox = collider.GetComponent<Script_HurtBox>();
        if (hurtBox != null)
        {
            int dmg = GetAttackStat().GetVal();
            print($"CollisionedWith with {hurtBox} inflicting dmg: {dmg}");
            hurtBox.Hurt(dmg);
            didHit = true;
        }
    }

    public override Model_Stat GetAttackStat()
    {
        return characterStats.stats.attack;
    }
}
