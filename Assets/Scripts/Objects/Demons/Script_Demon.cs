using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Demon : MonoBehaviour
{
    public Script_Game game;
    public int Id;
    public Model_Thought thought;

    // Update is called once per frame
    void Update()
    {
        AdjustRotation();    
    }

    public virtual void DefaultAction()
    {
        game.EatDemon(Id);
        game.AddPlayerThought(thought);
        game.ShowAndCloseThought(thought);
    }

    public void AdjustRotation()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public virtual void Setup(Model_Thought _thought, Sprite sprite)
    {
        game = FindObjectOfType<Script_Game>();
        GetComponent<SpriteRenderer>().sprite = sprite;
        thought = _thought;

        AdjustRotation();
    }
}
