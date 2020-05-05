using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model_NPC
{
    public Vector3 NPCSpawnLocation;
    public Sprite sprite;
    public Model_Dialogue dialogue;

    /*
        MovingNPCs
    */
    public bool isMovingNPC = false;
    public bool isCutSceneNPC;
    public Model_MoveSet[] moveSets;
    // needed to tell movingNPC animator where to start idle
    public string direction;
    public string type;
}
