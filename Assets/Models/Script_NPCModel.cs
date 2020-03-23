using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Script_NPCModel
{
    public Vector3 NPCSpawnLocation;
    public Sprite sprite;
    public Script_Dialogue dialogue;

    // TODO: add optional setmoves for MovingNPC
    public bool isMovingNPC = false;
    public Script_MoveSetModel[] moveSets;
}
