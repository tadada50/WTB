using System.Collections.Generic;
using UnityEngine;
public enum MysteryBoxAbilitys
{

    BombTimeModifier,
    BombTimeReveal,
   // SpeedModifier,
   // SpeedModifierAndBombTimeModifier,
   // SpeedModifierAndBombTimeModifierAndExtraLife,
    // ExtraLife
}

public class MysteryBox : GadgetBehavior
{
    // [SerializeField] Sprite activatedSprite;
    // [SerializeField] Sprite neutralSprite;
    // [SerializeField] float bombTimeModifier = 5.0f;
    // [SerializeField] LevelManager levelManager;
    // [SerializeField] int activationLimit = 1;

    [SerializeField] List<GameObject> reWardTypes;
    // int activationTimes = 0;

    // bool isActive = true;

    //bool isPressed = false;

    // private string[] validTags = { "RightPlayer", "LeftPlayer", "Bomb" };  // Add or modify tags as needed
    
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if(isActive == false){
    //         return;
    //     }
    //     if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
    //     {
    //       //  isPressed = true;
    //         GetComponent<SpriteRenderer>().sprite = activatedSprite;
    //         ActivateGadgetFunction();

    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     if(isActive == false){
    //         return;
    //     }
    //     if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
    //     {
    //       //  isPressed = false;
    //         GetComponent<SpriteRenderer>().sprite = neutralSprite;
    //     }
    // }

    public override void ActivateGadgetFunction()
    {
        MysteryBoxAbilitys randomAbility = (MysteryBoxAbilitys)Random.Range(0, System.Enum.GetValues(typeof(MysteryBoxAbilitys)).Length);
        activationTimes++;
        if(activationTimes>activationLimit){
            return;
        }
        if(randomAbility == MysteryBoxAbilitys.BombTimeModifier)
        {
            bombTimeModifier = Random.Range(-10, 11);
            levelManager.AddTimeToBomb(bombTimeModifier);
        }
        else if (randomAbility == MysteryBoxAbilitys.BombTimeReveal)
        {
            // ActivateBombTimeReveal();
            levelManager.RevealBombTime();
        }
        Debug.Log($"Mystery box activated with ability: {randomAbility}");
        
    }
    // void UpdateGadgetState()
    // {
    //     if (activationTimes >= activationLimit)
    //     {
    //         isActive = false;
    //         GetComponent<SpriteRenderer>().sprite = neutralSprite;
    //         gameObject.GetComponent<SpriteRenderer>().enabled = false;

    //     }

    // }
    // public void ActivateGadget()
    // {
    //     // Debug.Log("Activating gadget");
    //     GetComponent<SpriteRenderer>().sprite = neutralSprite;
    //     isActive = true;
    //     activationTimes = 0;
    //     gameObject.GetComponent<SpriteRenderer>().enabled = true;
    // }

}
