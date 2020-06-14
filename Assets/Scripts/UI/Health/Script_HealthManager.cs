using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_HealthManager : MonoBehaviour
{
    public Vector3 heartSpawnLocation;
    public Script_HealthHeart heartPrefab;
    public Script_HealthHeartsHolder heartContainer;
    public CanvasGroup healthCanvas;
    

    [SerializeField]
    private List<Script_HealthHeart> hearts;
    // if this goes above slots - 1, then player loses
    [SerializeField]
    private int heartIndex;
    [SerializeField]
    private Script_Game game;
    

    public void FillHearts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            hearts[heartIndex].Fill();
            heartIndex++;

            // check for death case
            if (heartIndex == hearts.Count)
            {
                print("DEATH!!!!!!!!!!!!!!!!!!");
            }
        }
    }

    void SetHeartChildren()
    {
        foreach (Script_PlayerThoughtsInventoryButton t in game.thoughtSlots)
        {
            Script_HealthHeart heart = Instantiate(heartPrefab, heartSpawnLocation, Quaternion.identity);
            heart.transform.SetParent(heartContainer.transform, false);
            hearts.Add(heart);
            heart.Setup(false);
        }
    }

    public void Close()
    {
        healthCanvas.GetComponent<Script_CanvasGroupController_Health>()
            .Close();
    }

    public void Open()
    {
        healthCanvas.GetComponent<Script_CanvasGroupController_Health>()
            .Close();
    }

    public void Setup()
    {
        // setup # of slots equal to thoughtSlots, start as empty state
        game = transform.parent.GetComponent<Script_Game>();
        SetHeartChildren();

        healthCanvas.gameObject.SetActive(true);        
    }
}
