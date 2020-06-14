using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Attack : MonoBehaviour, IHitBoxResponder
{
    [SerializeField] protected Script_HitBox activeHitBox;
    [SerializeField] protected Script_HitBox hitBoxN;
    [SerializeField] protected Script_HitBox hitBoxE;
    [SerializeField] protected Script_HitBox hitBoxS;
    [SerializeField] protected Script_HitBox hitBoxW;
    [SerializeField] protected Script_CharacterStats characterStats;
    
    public virtual void Attack(string dir)
    {
        activeHitBox = GetHitBoxDirection(dir);
        
        activeHitBox.SetResponder(this);
        activeHitBox.StartCheckingCollision();
    }

    Script_HitBox GetHitBoxDirection(string dir)
    {
        if (dir == Const_Directions.Up)            return hitBoxN;
        else if (dir == Const_Directions.Right)    return hitBoxE;
        else if (dir == Const_Directions.Down)     return hitBoxS;
        else                                       return hitBoxW;
    }

    public virtual void EndAttack()
    {
        activeHitBox.StopCheckingCollision();
    }
    
    /// <summary>
    /// via the interface, this handles the hurtbox colliding
    /// </summary>
    /// <param name="collider">the hurtbox</param>
    public virtual void CollisionedWith(Collider collider) {
        // Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        // hurtbox?.getHitBy(damage);
    }

    public virtual Model_Stat GetAttackStat()
    {
        return characterStats.stats.attack;
    }
}
