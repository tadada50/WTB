using UnityEngine;
using UnityEngine.EventSystems;

public class RightThrowControl : EventTrigger, ISelectHandler
{
    PlayerMovement player;
    public override void OnPointerEnter(PointerEventData data)
    {
        // player.InitiateThrow2();
        // // Debug.Log("OnPointerEnter called.");
        // // // Debug.Log("Jump selected");
        // if(player == null){
        //     player = GameObject.FindGameObjectWithTag("RightPlayer").GetComponent<PlayerMovement>();
        //     player.InitiateThrow2();
        //     // player.Jump();
        // }else{
        //     player.InitiateThrow2();
        //     // player.Jump();
        // }
    
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(player == null){
            player = GameObject.FindGameObjectWithTag("RightPlayer").GetComponent<PlayerMovement>();
            player.InitiateThrow2();
            // player.Jump();
        }else{
            player.InitiateThrow2();
            // player.Jump();
        }
    
    }


    //     public override void OnBeginDrag(PointerEventData data)
    // {
    //     Debug.Log("OnBeginDrag called.");
    // }

    // public override void OnCancel(BaseEventData data)
    // {
    //     Debug.Log("OnCancel called.");
    // }

    // public override void OnDeselect(BaseEventData data)
    // {
    //     Debug.Log("OnDeselect called.");
    // }

    // public override void OnDrag(PointerEventData data)
    // {
    //     Debug.Log("OnDrag called.");
    // }

    // public override void OnDrop(PointerEventData data)
    // {
    //     Debug.Log("OnDrop called.");
    // }

    // public override void OnEndDrag(PointerEventData data)
    // {
    //     Debug.Log("OnEndDrag called.");
    // }

    // public override void OnInitializePotentialDrag(PointerEventData data)
    // {
    //     Debug.Log("OnInitializePotentialDrag called.");
    // }

    // public override void OnMove(AxisEventData data)
    // {
    //     Debug.Log("OnMove called.");
    // }

    // public override void OnPointerClick(PointerEventData data)
    // {
    //     Debug.Log("OnPointerClick called.");
    // }

    // public override void OnPointerDown(PointerEventData data)
    // {
    //     Debug.Log("OnPointerDown called.");
    // }

    public override void OnPointerExit(PointerEventData data)
    {
        // if(player == null){
        //     player = FindFirstObjectByType<Player1>();
        //     player.isThrottling = false;
        //     // player.Jump();
        // }else{
        //     player.isThrottling = false;
        //     // player.Jump();
        // }

        if(player == null){
            player = GameObject.FindGameObjectWithTag("RightPlayer").GetComponent<PlayerMovement>();
            player.ThrowControlFinish();
            // player.Jump();
        }else{
            player.ThrowControlFinish();
            // player.Jump();
        }
        

    }

    // public override void OnPointerUp(PointerEventData data)
    // {
    //     Debug.Log("OnPointerUp called.");
    // }

    // public override void OnScroll(PointerEventData data)
    // {
    //     Debug.Log("OnScroll called.");
    // }

    // public override void OnSelect(BaseEventData data)
    // {
    //     Debug.Log("OnSelect called.");
    // }

    // public override void OnSubmit(BaseEventData data)
    // {
    //     Debug.Log("OnSubmit called.");
    // }

    // public override void OnUpdateSelected(BaseEventData data)
    // {
    //     Debug.Log("OnUpdateSelected called.");
    // }
}
