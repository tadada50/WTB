using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float maxVelocity = 15f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform hand;
    [SerializeField] bool isRightSide = true;
    [SerializeField] bool isTouchControl = false;
    [SerializeField] float touchMoveMultiplier = 0.5f;
    // [SerializeField] float maxGravity = 20f;
    Rigidbody2D myRigidbody;
    bool isAlive = true;
    bool isActive = false;
    Vector2 moveInput;
    Vector2 throwInput;
    Animator myAnimator;
    Vector2 screenBounds;
    float playerHalfWidth;
    float playerHalfHeight;
    [SerializeField] float throwForce = 100f; // Force applied to the bullet when thrown
    Bomb bomb;
    KeyCode attackKey = KeyCode.Space; // Key to trigger the attack/throw action
    KeyCode upKey = KeyCode.W; // Key to increase the Y component of the throw force
    KeyCode downKey = KeyCode.S; // Key to decrease the Y component of the throw force

    public Joystick joystick;
    public Joystick throwJoystick;

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
        if(isRightSide){
            attackKey = KeyCode.RightControl;
            upKey = KeyCode.UpArrow; // Change to Up Arrow for increasing Y component of the throw force
            downKey = KeyCode.DownArrow; // Change to Down Arrow for decreasing Y component of the throw force
        }else{
            attackKey = KeyCode.Space; // Change to Left Control for left side player
            upKey = KeyCode.W; // Up Arrow for increasing Y component of the throw force
            downKey = KeyCode.S; // Down Arrow for decreasing Y component of the throw force
        }

        throwJoystick.OnDragFinish += DragFinishHandler;

    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        AccumulateThrowForce();
        RunWithJoystick();
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
    
    public void ThrowBombWithTouch(Vector2 startPos, Vector2 endPos){
        if(!isActive || bomb == null) // Only throw if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        Debug.Log($"StartPos:{startPos} EndPos:{endPos}" );
        //  bomb.Throw(force, 1f);
        //  bomb = null;
    }

    void DragFinishHandler(float duration){
        if (!isActive || bomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        
        Vector2 force;
        force.x = throwJoystick.lastNonZeroHorizontal * duration * 15f;
        force.y = throwJoystick.lastNonZeroVertical * duration * 15f;

        Debug.Log($"DragFinished throw force:{force}  lastNonZeroVertical:{throwJoystick.lastNonZeroVertical}   lastNonZeroHorizontal:{throwJoystick.lastNonZeroHorizontal}");
        bomb.transform.SetParent(null); // Detach from player
        bomb.Throw(force,1f);
        bomb = null;
    }
    void ThrowWithTouch(){
        if (!isActive || bomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if(!isTouchControl){
            return;
        }
        float x = 0f;
        float y = 0f;
        // if(throwJoystick.Horizontal >= 0.2f){
        //     x = 1;
        // }else if(throwJoystick.Horizontal<=-0.2f){
        //     x = -1;
        // }
        // if(throwJoystick.Vertical >= 0.2f){
        //     y = 1;
        // }else if(throwJoystick.Vertical<=-0.2f){
        //     y = -1;
        // }
        throwInput = new Vector2(throwJoystick.Horizontal,throwJoystick.Vertical);

    }
    void AccumulateThrowForce(){
        // Check if the player has pressed the throw button (e.g., left mouse button or a specific key)
        if (!isActive || bomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if (Input.GetKey(attackKey)){

            if(Input.GetKey(upKey)){
                throwForceVector2.y += Time.deltaTime * 12f; // Increase the Y component of the throw force when W is pressed (upward)
            }
            if(Input.GetKey(downKey)){
                throwForceVector2.y -= Time.deltaTime * 12f; // Increase the Y component of the throw force when W is pressed (upward)
            }

            // if(Input.GetKey(KeyCode.A)){
            //     throwForceVector2.x -= Time.deltaTime * 15f; // Increase the Y component of the throw force when W is pressed (upward)
            // }
            isThrowing = true;
            throwForceVector2.x += Time.deltaTime * 17f; // Update initial position based on the throw force (for example, you can use mouse position or a fixed offset)
            

        }
        if (Input.GetKeyUp(attackKey))
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
        // Debug.Log("OnMove called with value: " + value);
        moveInput = value.Get<Vector2>();
        // if(isThrowing){
        //     // Debug.Log($"Throwing in progress: {throwForceVector2}");
        //     moveInput = Vector2.zero; // Prevent movement input while throwing the bomb
        // }
       // transform.Translate(moveInput.x * Time.deltaTime, 0, moveInput.y * Time.deltaTime);
      //  transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
        // backGround.transform.Translate(moveInput.x * Time.deltaTime, moveInput.y * Time.deltaTime, 0);
    }
    public void RunWithJoystick(){
        if(!isTouchControl){
            return;
        }
        if(!isAlive || !isActive){
            return;
        }
        float x = 0f;
        float y = 0f;
        if(joystick.Horizontal >= 0.2f){
            x = 1;
        }else if(joystick.Horizontal<=-0.2f){
            x = -1;
        }
        if(joystick.Vertical >= 0.2f){
            y = 1;
        }else if(joystick.Vertical<=-0.2f){
            y = -1;
        }
        moveInput = new Vector2(x,y);
    
    }
    public void StopMoving(){
        moveInput = Vector2.zero;
        Debug.Log($"StopMoving ===> isRightPlayer:{isRightSide} moveInput:{moveInput}");
    }
    public void RunWithTouch(Vector2 touchPosition){
        
        // This method can be called from touch input to handle player movement with touch controls
        // if(!isAlive || !isActive){
        //     return;
        // }
        // Calculate movement direction based on touch position relative to player
        // Vector2 moveDirection = ((Vector2)touchPosition - (Vector2)transform.position).normalized;
        // Vector2 moveDirection = ((Vector2)Camera.main.ScreenToWorldPoint(transform.position) - (Vector2)touchPosition).normalized;
        // Vector2 moveDirection = ((Vector2)touchPosition - (Vector2)transform.position).normalized;


        // Vector2 moveDirection = (Vector2)touchPosition - (Vector2)transform.position;
        // moveInput = moveDirection; // Set moveInput for use in Run method
        // Debug.Log($"isRightPlayer:{isRightSide} moveInput:{moveInput} touchposition:{touchPosition} transform.position:{transform.position}");


        // Debug.Log("Running with touch input");
        // Assuming moveInput is already set based on touch input
       // Run(); // Call the Run method to apply movement based on the current moveInput
    }
    void Run(){
        if(isThrowing || !isAlive || !isActive){
            return;
        }

        // if (Input.GetKey(KeyCode.Space)){
        //     Debug.Log($"Space key is pressed, isThrowing: {isThrowing}, isActive: {isActive}, bomb: {(bomb != null ? "present" : "null")}");
        //     return;
        // }
       // Debug.Log("Running"); 
       Vector2 playVelocity;
       if(isTouchControl){
            playVelocity = new Vector2(moveInput.x * runSpeed, moveInput.y * runSpeed);
       }else{
            playVelocity = new Vector2(moveInput.x * runSpeed*touchMoveMultiplier, moveInput.y * runSpeed*touchMoveMultiplier);
       }
    
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
            // Debug.Log("Point Velocity Y : " + myRigidbody.GetPointVelocity(myRigidbody.position).y);
            if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(myRigidbody.linearVelocityX)){
                if(bomb!=null){
                    bomb.FlipBombText(); // Flip the bomb text if the player is facing a different direction
                }
            }
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocityX), 1f);

        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("OnTriggerEnter2D collision with: " + collision.tag);
        if(collision.tag == "Bomb" && bomb==null){
            bomb = collision.gameObject.GetComponent<Bomb>();
            if(bomb != null && !bomb.GetComponent<Bomb>().hasOwner) { 
            //if(bomb != null && !bomb.GetComponent<Bomb>().hasOwner && !bomb.beingThrown) {  // Check if the bomb has no owner
                bomb.SetHasOwner(true);
                bomb.beingThrown = false;
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
