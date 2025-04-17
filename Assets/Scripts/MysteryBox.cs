using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] List<GadgetBehavior> reWardTypes;
    [SerializeField] bool isRightSide = false;
    // int activationTimes = 0;

    // bool isActive = true;

    //bool isPressed = false;

    // private string[] validTags = { "RightPlayer", "LeftPlayer", "Bomb" };  // Add or modify tags as needed
    
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetType() != typeof(BoxCollider2D)) return;
        Debug.Log("OnTriggerEnter2D called. From MysteryBox");
        if(isActive == false){
            return;
        }
        if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
        {
          //  isPressed = true;
            GetComponent<SpriteRenderer>().sprite = activatedSprite;
            ActivateGadgetFunction(isRightSide);

        }
    }

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



    public override void ActivateGadgetFunction(bool isRightSide)
    {
        // Debug.Log("==> ActivateGadgetFunction activated.");
        MysteryBoxAbilitys randomAbility = (MysteryBoxAbilitys)Random.Range(0, System.Enum.GetValues(typeof(MysteryBoxAbilitys)).Length);
        activationTimes++;
        if(activationTimes>activationLimit){
            Debug.Log("Activation limit reached. Exiting function.");
            return;
        }
        
        int randomRewardIndex = Random.Range(0, reWardTypes.Count);
        // Debug.Log($"Random reward index: {randomRewardIndex}");
        // Debug.Log($"Random reward: {reWardTypes[randomRewardIndex]}");
        if(reWardTypes[randomRewardIndex] != null)
        {
         //   Debug.Log($"Adding random gadget: {reWardTypes[randomRewardIndex]}  isRightSide: {isRightSide}");
            levelManager.AddRandomGadget(reWardTypes[randomRewardIndex], isRightSide);
            // if(reWardTypes[randomRewardIndex] is RevelationGadget)
            // {
            //     Debug.Log("RevelationGadget activated.");
            //     // Instantiate(reWardTypes[randomRewardIndex], transform.position, Quaternion.identity);
            //     levelManager.AddRandomGadget(reWardTypes[randomRewardIndex], isRightSide);
            // }else if(reWardTypes[randomRewardIndex] is GadgetBehavior)
            // {
            //     reWardTypes[randomRewardIndex].SetBombTimeModifier( Random.Range(-10, 11));
            //     levelManager.AddRandomGadget(reWardTypes[randomRewardIndex], isRightSide);
            //     // Instantiate(reWardTypes[randomRewardIndex], transform.position, Quaternion.identity);
            // }
        // Instantiate(reWardTypes[randomRewardIndex], transform.position, Quaternion.identity);

        // if(randomAbility == MysteryBoxAbilitys.BombTimeModifier)
        // {
        //     bombTimeModifier = Random.Range(-10, 11);
        //     levelManager.AddTimeToBomb(bombTimeModifier);
        // }
        // else if (randomAbility == MysteryBoxAbilitys.BombTimeReveal)
        // {
        //     // ActivateBombTimeReveal();
        //     levelManager.RevealBombTime();
        // }
        // Debug.Log($"Mystery box activated with ability: {randomAbility}");
        
        }
    }




    // public override void ActivateGadgetFunction()
    // {
    //     MysteryBoxAbilitys randomAbility = (MysteryBoxAbilitys)Random.Range(0, System.Enum.GetValues(typeof(MysteryBoxAbilitys)).Length);
    //     activationTimes++;
    //     if(activationTimes>activationLimit){
    //         return;
    //     }
    //     if(randomAbility == MysteryBoxAbilitys.BombTimeModifier)
    //     {
    //         bombTimeModifier = Random.Range(-10, 11);
    //         levelManager.AddTimeToBomb(bombTimeModifier);
    //     }
    //     else if (randomAbility == MysteryBoxAbilitys.BombTimeReveal)
    //     {
    //         // ActivateBombTimeReveal();
    //         levelManager.RevealBombTime();
    //     }
    //     Debug.Log($"Mystery box activated with ability: {randomAbility}");
        
    // }
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
