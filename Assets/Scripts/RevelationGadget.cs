using UnityEngine;

public class RevelationGadget : GadgetBehavior
{
    public override void ActivateGadgetFunction(bool isRightSide)
    {
        activationTimes++;
        if(levelManager == null){
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }
        levelManager.RevealBombTime();
    }
}
