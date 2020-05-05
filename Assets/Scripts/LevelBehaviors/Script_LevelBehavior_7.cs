using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LevelBehavior_7 : Script_LevelBehavior
{
    public bool isDone = false;
    public bool isActivated = false;
    
    
    public float demonRevealWaitTime;
    public float postDemonRevealWaitTime;
    public float cagesRevealWaitTime;
    public float postCagesRevealWaitTime;
    public float postFinalWordsWaitTime;
    public float spotlightVolumeScale;
    public float zoomOutDemonsSize;
    public float zoomOutCagesSize;
    
    public AudioSource audioSource;
    public AudioClip SpotlightSFX;
    public GameObject MelzSpotlight;
    public GameObject demonsAndSpotlights;
    public GameObject demonSpotLights;
    public GameObject cages;
    public Script_DialogueManager dm;
    public Script_DialogueNode ateDemonsNode;
    public Script_DialogueNode sparedDemonsNode;
    public Script_DialogueNode showDemonsNode;
    public Script_DialogueNode showCagesNode;
    public Script_DialogueNode finalWordsNode;
    public Script_LevelBehavior_3 level3;


    private bool didEatDemons = false;
    private bool revealDemonsAndSpotlights = false;
    private bool revealCages = false;
    private bool finalWords = false;
    
    protected override void OnDisable() {
        print("return camera to default");
        if (game.camera != null)
        {
            game.SetOrthographicSizeDefault();
        }
    }
    
    protected override void HandleOnEntrance()
    {
        if (game.state == "interact" && !isActivated)
        {
            print("PLAYER CAN INTERACT");
            isActivated = true;

            
            foreach(bool demonSpawn in level3.demonSpawns)
            {
                if (!demonSpawn)    didEatDemons = true;
            }

            if (didEatDemons)   dm.StartDialogueNode(ateDemonsNode);
            else                dm.StartDialogueNode(sparedDemonsNode);
            
            game.ChangeStateCutScene();
        }

        // allow player to press Inventory Open on Last DialogueSection
        // after finishing typing line
        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "inventory-open")
        )
        {
            HandleInventoryOpenAndClose();
            return;
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "reveal-demons")
            && !revealDemonsAndSpotlights
        )
        {
            revealDemonsAndSpotlights = true;
            StartCoroutine(WaitToRevealDemons());
            return;
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "reveal-cages")
            && !revealCages
        )
        {
            revealCages = true;
            StartCoroutine(WaitToRevealCages());
            return;
        }

        if (
            Script_Utils.CheckLastNodeActionCutScene(game, dm, "final-words")
            && !finalWords
        )
        {
            finalWords = true;
            StartCoroutine(WaitToMelzExit());
            return;
        }
    }

    IEnumerator WaitToRevealDemons()
    {
        yield return new WaitForSeconds(demonRevealWaitTime);
        
        audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
        game.SetOrthographicSize(zoomOutDemonsSize);
        // game.ChangeCameraTargetToNPC(0);
        demonsAndSpotlights.SetActive(true);
        demonSpotLights.SetActive(true);

        yield return new WaitForSeconds(postDemonRevealWaitTime);
        dm.StartDialogueNode(showCagesNode, false);
    }

    IEnumerator WaitToRevealCages()
    {
        yield return new WaitForSeconds(cagesRevealWaitTime);
        
        audioSource.PlayOneShot(SpotlightSFX, spotlightVolumeScale);
        game.SetOrthographicSize(zoomOutCagesSize);
        cages.SetActive(true);
        demonSpotLights.SetActive(false);

        yield return new WaitForSeconds(postCagesRevealWaitTime);
        dm.StartDialogueNode(finalWordsNode, false);
    }

    IEnumerator WaitToMelzExit()
    {
        yield return new WaitForSeconds(postFinalWordsWaitTime);
        
        MelzExit();
    }

    void ChangeGameStateInteract()
    {
        game.ChangeStateInteract();
    }

    void MelzExit()
    {
        game.DestroyCutSceneNPC(0);
        MelzSpotlight.SetActive(false);
        dm.EndDialogue();
        game.ChangeStateInteract();
        
        isDone = true;
    }

    // handle inventory here since player is still in talking mode
    void HandleInventoryOpenAndClose()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (!game.GetIsInventoryOpen())
            {
                game.OpenPlayerThoughtsInventory();
            }
            else
            {
                game.ClosePlayerThoughtsInventory();
                // trigger second part of dialogue
                dm.StartDialogueNode(showDemonsNode, false);
            }
        }
        else if (Input.GetButtonDown("Cancel") && game.GetIsInventoryOpen())
        {
            game.ClosePlayerThoughtsInventory();
            // trigger second part of dialogue
            dm.StartDialogueNode(showDemonsNode, false);
        }
    }

    protected override void HandleAction()
    {
        // don't allow dialogue actions on dialogue section where
        // player is learning to open inventory
        if (
            game.state == "cut-scene"
            && game.GetPlayerIsTalking()
            && dm.dialogueSections.Count == 0
            && dm.lines.Count == 0
        )
        {
            if (
                dm.currentNode.data.action == "inventory-open"
                || dm.currentNode.data.action == "reveal-demons"
                || dm.currentNode.data.action == "reveal-cages"
                || dm.currentNode.data.action == "final-words"
            )
            {
                return;
            }
        }
        
        base.HandleDialogueAction();
    }

    public override void Setup()
    {
        if (!isDone)
        {
            game.CreateNPCs();
            demonsAndSpotlights.SetActive(false);
            demonSpotLights.SetActive(false);
            cages.SetActive(false);
        }
        else
        {
            demonsAndSpotlights.SetActive(true);
            demonSpotLights.SetActive(false);
            cages.SetActive(true);
        }
    }
}
