using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DemonRenderer : MonoBehaviour
{
    [SerializeField] private Script_Demon parent;

    /* =============================================================        
        ANIMATION FUNCS BEGIN: called from animation
    ============================================================= */
    // called from: Demon_Default_swallowed-heart-ending
    private void FinishSwallowed()
    {
        parent.FinishSwallowed();
    }
    /* =============================================================    
        END
    ============================================================= */
}
