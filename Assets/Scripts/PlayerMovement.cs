using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float maxVelocity = 15f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform hand;
    [SerializeField] bool isRightSide = true;
    // [SerializeField] float maxGravity = 20f;
    Rigidbody2D myRigidbody;
    bool isAlive = true;
    bool isActive = false;
    Vector2 moveInput;
    Animator myAnimator;
    Vector2 screenBounds;
    float playerHalfWidth;
    float playerHalfHeight;
    Bomb bomb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHalfHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        Run();   
    }
    
    public void SetActive(bool isActive){
        this.isActive = isActive;
        // if(!isActive){
        //     myRigidbody.velocity = Vector2.zero;
        //     myAnimator.SetBool("isRunning", false);
        //     myAnimator.SetBool("isJumping", false);
        // }
    }
    void FixedUpdate()
    {
        // if (myRigidbody.linearVelocity.magnitude > maxVelocity)
        // {
        //     myRigidbody.linearVelocity = myRigidbody.linearVelocity.normalized * maxVelocity;
        // }
    }
    void OnMove(InputValue value){
        if(!isAlive || !isActive){
            return;
        }
        // Debug.Log("OnMove called with value: " + value);
        moveInput = value.Get<Vector2>();
       // transform.Translate(moveInput.x * Time.deltaTime, 0, moveInput.y * Time.deltaTime);
        transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
        // backGround.transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
    }
    void Run(){
        Vector2 playVelocity = new Vector2(moveInput.x * runSpeed, moveInput.y * runSpeed);
        // Vector2 playVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playVelocity;
        myAnimator.SetBool("isRunning", Mathf.Abs(myRigidbody.linearVelocityX) > Mathf.Epsilon || Mathf.Abs(myRigidbody.linearVelocityY) > Mathf.Epsilon);
        float clampedX;
        float clampedY;
        if(isRightSide){
            clampedX = Mathf.Clamp(myRigidbody.position.x, 0 + playerHalfWidth, screenBounds.x - playerHalfWidth);
        }else{
            clampedX = Mathf.Clamp(myRigidbody.position.x, -screenBounds.x + playerHalfWidth, 0 - playerHalfWidth);
        }
        // clampedX = Mathf.Clamp(myRigidbody.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);
        clampedY = Mathf.Clamp(myRigidbody.position.y, -screenBounds.y + playerHalfHeight, screenBounds.y - playerHalfHeight);
        Vector2 pos = transform.position;
        pos.x = clampedX;
        pos.y = clampedY;
        transform.position = pos;
        //carry the bomb with the player
        // if (bomb != null)
        // {
        //     Vector3 position = new Vector3(playerHalfWidth * Mathf.Sign(transform.localScale.x), 0, 0);
        //     bomb.transform.localPosition = position;
        // }
        // Vector2 backGroundVelocity = new Vector2(moveInput.x * runSpeed * backGroundMoveFactorX, myRigidbody.linearVelocity.y* backGroundMoveFactorY);
        // backgroundRigitBody.linearVelocity = backGroundVelocity;
        // myAnimator.SetBool("isJumping", myRigidbody.linearVelocity.y > Mathf.Epsilon);
    }
    void FlipSprite(){
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocityX) > 0.05;
       // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.GetPointVelocity().x) > Mathf.Epsilon;



        if (playerHasHorizontalSpeed)
        {
            // Debug.Log("-->Linear Velocity X (ABS): " + Mathf.Abs(myRigidbody.linearVelocityX));
            // Debug.Log("HasHorizontalSpeed : " + playerHasHorizontalSpeed);
            // Debug.Log("<--Point Velocity X : " + myRigidbody.GetPointVelocity(myRigidbody.position).x);
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocityX), 1f);
            
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("OnTriggerEnter2D collision with: " + collision.tag);
        if(collision.tag == "Bomb" && bomb==null){
            bomb = collision.gameObject.GetComponent<Bomb>();
            if(bomb != null && !bomb.GetComponent<Bomb>().hasOwner) {  // Check if the bomb has no owner
                bomb.SetHasOwner(true);
                PickupBomb(bomb);
                // bomb.AddTime(5f); // Optional: Add time to the bomb countdown
            }else{
                bomb = null;
            }
            //wasCollected = true;
            //AudioSource.PlayClipAtPoint(coinPickupSFX,Camera.main.transform.position);
            //gameSession.AddToScore(pointsForPickup);
            //Destroy(gameObject);
        }
    }
    void PickupBomb(Bomb bomb)
    {
        // Add logic to handle bomb pickup here
        bomb.transform.SetParent(hand.transform);
        Vector3 position = new Vector3(0, 0, 0);
        bomb.transform.localPosition = position;
        // bomb.transform.localPosition = new Vector3(playerHalfWidth/2, playerHalfHeight/2, 0); // Adjust position relative to player
        // For example, you might want to change the player's state or update UI
        // Debug.Log("Bomb picked up! Countdown: " + bomb.GetCountdown());
        // You can also destroy the bomb object if needed
        // UnityEngine.Object.Destroy(bomb.gameObject); // Destroy the bomb after pickup
    }
    
}
