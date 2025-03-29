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
    [SerializeField] float throwForce = 100f; // Force applied to the bullet when thrown
    Bomb bomb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHalfHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        // Print the screen bounds corners in world coordinates
        Vector2 topRight = new Vector2(screenBounds.x, screenBounds.y);
        Vector2 topLeft = new Vector2(-screenBounds.x, screenBounds.y);
        Vector2 bottomRight = new Vector2(screenBounds.x, -screenBounds.y);
        Vector2 bottomLeft = new Vector2(-screenBounds.x, -screenBounds.y);

        Debug.Log($"Top Right: {topRight}");
        Debug.Log($"Top Left: {topLeft}"); 
        Debug.Log($"Bottom Right: {bottomRight}");
        Debug.Log($"Bottom Left: {bottomLeft}");
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        AccumulateThrowForce();
        Run();   
    }
    void OnAttack(){
        
        // This method can be called from an input action to handle the attack/throw action
        // Check if the player has pressed the attack button (e.g., left mouse button or a specific key)
        // if (isActive && bomb != null)
        // {
        //     AccumulateThrowForce();
        // }else{
        //     throwForce = 10f;
        // }
        // You can add additional logic here for other attack-related actions if needed 
    }
    Vector2 throwForceVector2 = Vector2.zero; // Initial position of the bomb when thrown
    Vector2 endPosition; // Final position of the bomb when thrown

    bool isThrowing = false; // Flag to check if the player is currently throwing the bomb
    void AccumulateThrowForce(){
        // Check if the player has pressed the throw button (e.g., left mouse button or a specific key)
        if (!isActive || bomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if (Input.GetKey(KeyCode.Space)){
            //throwForce += Time.deltaTime * 5f; // Increase throw force over time while the space key is held down
            // Start accumulating throw force when the space key is pressed
            // You can add logic here to indicate that the player is charging the throw
           // Debug.Log("Charging throw force... "+ throwForce);
            
            // if(Input.GetKey(KeyCode.W)){
            //     throwForceVector2.y += Time.deltaTime * 15f; // Increase the Y component of the throw force when W is pressed (upward)
            // }
            // if(Input.GetKey(KeyCode.S)){
            //     throwForceVector2.y -= Time.deltaTime * 15f; // Increase the Y component of the throw force when W is pressed (upward)
            // }

            // if(Input.GetKey(KeyCode.A)){
            //     throwForceVector2.X -= Time.deltaTime * 15f; // Increase the Y component of the throw force when W is pressed (upward)
            // }
            isThrowing = true;
            throwForceVector2.x += Time.deltaTime * 15f; // Update initial position based on the throw force (for example, you can use mouse position or a fixed offset)
            

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //Debug.Log("Space key was released."+ throwForce);
            //Throw the bomb
            Vector2 throwDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            
            bomb.transform.SetParent(null); // Detach from player

            
            // = GameObject.FindWithTag("TestLandingTransform").transform;
            Vector2 topLeft = new Vector2(-screenBounds.x, screenBounds.y);
            Vector2 force = throwForceVector2;
            force.x = transform.localScale.x * force.x; 
            // force.x = Mathf.Clamp(force.x, -screenBounds.x, screenBounds.x);
            // force.y = Mathf.Clamp(force.y, -screenBounds.y, screenBounds.y);
           // landingTarget.position = topLeft;
            // Vector2 landingPosition = landingTarget != null ? landingTarget.position : (Vector2)transform.position + throwDirection * 5f;
            bomb.Throw(force, 1f);
            bomb = null; // Reset bomb reference
            throwForce = 10f; // Reset throw force
            throwForceVector2 = Vector2.zero;
            isThrowing = false; // Reset the throwing flag
        }


        // if (Mouse.current.leftButton.wasPressedThisFrame && isActive && bomb != null)
        // {
        //     // Create a bullet instance at the player's position
        //     GameObject bulletInstance = Instantiate(bullet, hand.position, Quaternion.identity);
        //     Rigidbody2D bulletRigidbody = bulletInstance.GetComponent<Rigidbody2D>();
            
        //     // Calculate the direction to throw the bullet based on player orientation
        //     Vector2 throwDirection = isRightSide ? Vector2.right : Vector2.left;
            
        //     // Apply force to the bullet in the direction of the throw
        //     bulletRigidbody.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
            
        //     // // Optionally, you can destroy the bomb after throwing it
        //     // Destroy(bomb.gameObject);
        //     // bomb = null; // Reset the bomb reference
        // }
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
        Debug.Log("OnMove called with value: " + value);
        moveInput = value.Get<Vector2>();
        if(isThrowing){
            Debug.Log($"Throwing in progress: {throwForceVector2}");
            moveInput = Vector2.zero; // Prevent movement input while throwing the bomb
        }
       // transform.Translate(moveInput.x * Time.deltaTime, 0, moveInput.y * Time.deltaTime);
      //  transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
        // backGround.transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
    }
    void Run(){
        if(isThrowing || !isAlive || !isActive){
            return;
        }
        if (Input.GetKey(KeyCode.Space)){
            Debug.Log($"Space key is pressed, isThrowing: {isThrowing}, isActive: {isActive}, bomb: {(bomb != null ? "present" : "null")}");
            return;
        }
       // Debug.Log("Running"); 
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
        //Debug.Log($"Player position adjusted to: {pos}");
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
            if(bomb != null && !bomb.GetComponent<Bomb>().hasOwner && !bomb.beingThrown) {  // Check if the bomb has no owner
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
