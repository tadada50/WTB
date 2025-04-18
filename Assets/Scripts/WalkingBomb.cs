using UnityEngine;

public class WalkingBomb : Bomb
{
    [SerializeField] public GameObject leftPlayerHome;
    [SerializeField] public GameObject rightPlayerHome;
    [SerializeField] public float speed = 5.0f; // Speed of the bomb
    Rigidbody2D rb;
    [SerializeField] Animator myAnimator;
    Vector2 nextWalkTarget;
    float standingTime = 1.0f;
    float standingTimer = 0.0f;
    

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        leftPlayerHome = GameObject.FindGameObjectWithTag("PlayerLeftHome");
        rightPlayerHome = GameObject.FindGameObjectWithTag("PlayerRightHome");
        // myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("isWalking", false);
        Debug.Log("WalkingBomb initialized.");
    }
    

    protected override void Update()
    {
        base.Update();
        ProcessHasOwner();
        Walk();
    }
    private void PickWalkDestination(){
        // clampedX = Mathf.Clamp(myRigidbody.position.x, playerHomeScript.homeTopLeft.x + playerHalfWidth, playerHomeScript.homeTopRight.x - playerHalfWidth);
        // clampedY = Mathf.Clamp(myRigidbody.position.y, playerHomeScript.homeBottomLeft.y + playerHalfHeight, playerHomeScript.homeTopLeft.y);



        float randomX = Random.Range(leftPlayerHome.GetComponent<PlayerHome>().homeTopLeft.x, rightPlayerHome.GetComponent<PlayerHome>().homeTopRight.x);
        float randomY = Random.Range(leftPlayerHome.GetComponent<PlayerHome>().homeBottomLeft.y, rightPlayerHome.GetComponent<PlayerHome>().homeTopLeft.y);
        nextWalkTarget = new Vector2(randomX, randomY);

    }
    private void ProcessHasOwner(){
        if(myAnimator == null){
            return;
        }
        if (hasOwner)
        {
            myAnimator.SetBool("isWalking", false);
            myAnimator.SetBool("hasOwner", true);
            // StopMoving();
            // return;
            // myAnimator.SetBool("isWalking", false);
            // myAnimator.SetBool("isStanding", true);
            // StopMoving();
            // return;
        }else{
            myAnimator.SetBool("hasOwner", false);
            // myAnimator.SetBool("isStanding", false);
            // myAnimator.SetBool("isWalking", true);
        }
        // myAnimator.SetBool("isWalking", false);
    }
    private void Walk()
    {
        if(beingThrown){
            PickWalkDestination();
            return;
        }
        if(exploded || hasOwner){
            StopMoving();
            return;
        }
        
        transform.rotation = Quaternion.identity; // Reset rotation to default
        if ((Vector2)rb.transform.position == nextWalkTarget)
        {
            if(myAnimator!= null){
                myAnimator.SetBool("isWalking", false);
            }
            // myAnimator.SetBool("isStanding", true);
            standingTime = Random.Range(0.8f, 2f);
            standingTimer += Time.deltaTime;
            if (standingTimer >= standingTime)
            {
                standingTimer = 0.0f;
                PickWalkDestination();
            }
            return;
            // PickWalkDestination();
        }
        if(myAnimator!= null){
                myAnimator.SetBool("isWalking", true);
        }
        // myAnimator.SetBool("isWalking", true);
        transform.position = Vector3.MoveTowards(transform.position, nextWalkTarget, speed * Time.deltaTime);
        // Flip sprite based on movement direction
        Vector2 direction = (nextWalkTarget - (Vector2)transform.position).normalized;
        float currentScaleX = transform.localScale.x;
        if (direction.x > 0){
            transform.localScale = new Vector3(1, 1, 1);
            if(currentScaleX < 0){
                FlipBombText();
            }
        } 
        else if (direction.x < 0){
            transform.localScale = new Vector3(-1, 1, 1);
            if(currentScaleX > 0){
                FlipBombText();
            }
        }

    }
    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        // myAnimator.SetBool("isRunning", false);
    }
}
