using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DDRHandler : MonoBehaviour
{
    public void HandleArrowClick(int tier, Script_LevelBehavior levelBehavior)
    {
        levelBehavior.HandleDDRArrowClick(tier);
    }
}
