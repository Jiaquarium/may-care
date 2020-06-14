using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PlayerAttacks : MonoBehaviour
{
    [SerializeField] private Script_PlayerAttackEat eatAttack;

    public void Eat(string direction)
    {
        eatAttack.Eat(direction);
    }
}
