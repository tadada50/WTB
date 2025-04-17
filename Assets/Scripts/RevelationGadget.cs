using UnityEngine;

public class RevelationGadget : GadgetBehavior
{
    public override void ActivateGadgetFunction()
    {
        activationTimes++;
        levelManager.RevealBombTime();
    }
}
