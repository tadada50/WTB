using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    // [SerializeField] float maxVelocity = 15f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform hand;
    [SerializeField] Transform bodyCenterOfGravity;
    [SerializeField] public bool isRightSide = true;
    [SerializeField] bool isTouchControl = false;
    [SerializeField] float touchMoveMultiplier = 0.4f;
    [SerializeField] public GameObject playerHome;
    [SerializeField] GameObject playerCenter;
    [SerializeField] GameObject throwForceSquare;
    [SerializeField] GameObject moveJoystickUI;
    [SerializeField] float throwMinTime = 0.02f;
    // [SerializeField] float maxGravity = 20f;
    Rigidbody2D myRigidbody;
    bool isAlive = true;
    bool isActive = false;
    bool isThrowing = false;
    Vector2 moveInput;
    Vector2 throwInput;
    Animator myAnimator;
    Vector2 screenBounds;
    float playerHalfWidth;
    float playerHalfHeight;
    [SerializeField] float throwForce = 100f; // Force applied to the bullet when thrown
    [SerializeField] GameObject characterBody;
    public Bomb mBomb;
    KeyCode attackKey = KeyCode.Space; // Key to trigger the attack/throw action
    KeyCode upKey = KeyCode.W; // Key to increase the Y component of the throw force
    KeyCode downKey = KeyCode.S; // Key to decrease the Y component of the throw force

    public Joystick joystick;
    public Joystick throwJoystick;

    public Button throwButton;
    public Vector2 homeTopLeft;
    public Vector2 homeBottomRight;
    bool throwInitiated = false;

    PlayerHome playerHomeScript;
    // [SerializeField] float runningAnimSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        if(myAnimator==null || myAnimator.runtimeAnimatorController==null){
            Debug.Log("Animator not found on PlayerMovement. Trying to find in children.");
            myAnimator = characterBody.GetComponent<Animator>();
        }
        lastBodyCenterOfGravityY = bodyCenterOfGravity.position.y;
        playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHalfHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        // Print the screen bounds corners in world coordinates
        Vector2 topRight = new Vector2(screenBounds.x, screenBounds.y);
        Vector2 topLeft = new Vector2(-screenBounds.x, screenBounds.y);
        Vector2 bottomRight = new Vector2(screenBounds.x, -screenBounds.y);
        Vector2 bottomLeft = new Vector2(-screenBounds.x, -screenBounds.y);

        playerHomeScript = playerHome.GetComponent<PlayerHome>();

        originalForceScalex = throwForceSquare.transform.localScale.x;

        // Debug.Log($"Top Right: {topRight}");
        // Debug.Log($"Top Left: {topLeft}"); 
        // Debug.Log($"Bottom Right: {bottomRight}");
        // Debug.Log($"Bottom Left: {bottomLeft}");
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
        throwJoystick.OnPointerDownEvent += InitiateThrow;

    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        HideThrowArrow();
        AccumulateThrowForce();
        UpdateThrowArrow2();
        RunWithJoystick();
        Run();   
        UpdateHasBombTrigger();
       // UpdateBombHeightRelativeToBodyCenterOfGravity();
    }
    void UpdateHasBombTrigger(){
        if(mBomb!=null){
            myAnimator.SetBool("hasBomb", true);
        }else{
            myAnimator.SetBool("hasBomb", false);
        }
    }
    float lastBodyCenterOfGravityY = 0f;
    void UpdateBombHeightRelativeToBodyCenterOfGravity(){
        if(mBomb==null || bodyCenterOfGravity==null){
            return;
        }
        Vector2 handPos = hand.transform.position;
        Vector2 bodyCenterOfGravityPos = bodyCenterOfGravity.position;
        float deltaY = bodyCenterOfGravityPos.y - lastBodyCenterOfGravityY;
        handPos.y += deltaY;
        hand.transform.position = handPos;
        lastBodyCenterOfGravityY = bodyCenterOfGravityPos.y;
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

    public void ThrowBombWithTouch(Vector2 startPos, Vector2 endPos){
        if(!isActive || mBomb == null) // Only throw if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        Debug.Log($"StartPos:{startPos} EndPos:{endPos}" );
        //  bomb.Throw(force, 1f);
        //  bomb = null;
    }
    float originalForceScalex;
    bool forceGrowing = true;
    void UpdateThrowArrow(){
        if (!isActive || mBomb == null || !isThrowing) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if(forceGrowing){
            throwForceTimer += Time.deltaTime; // Increment the throw force timer
        }else{
            throwForceTimer -= Time.deltaTime; // Decrement the throw force timer
        } // Increment the throw force timer
        if(throwForceTimer > maxThrowAccumulationTime)
        {
            forceGrowing = false; // Clamp the timer to the maximum accumulation time
        }
        if(throwForceTimer <= 0f){
            forceGrowing = true; // Reset the timer if it goes below zero
        }
        float forcePercentage = throwForceTimer / maxThrowAccumulationTime; // Calculate the force percentage based on the timer
        throwForceSquare.transform.localScale = new Vector3(originalForceScalex*(1+2.5f*forcePercentage), throwForceSquare.transform.localScale.y, throwForceSquare.transform.localScale.z); 
        Vector2 direction = throwJoystick.Direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Debug.Log($"Throw angle: {angle}  ForceSquare:{throwForceSquare.transform.localScale.x} ThrowForceTimer:{throwForceTimer}");
        playerCenter.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        
    }

    void UpdateThrowArrow2(){
        if (!isActive || mBomb == null || !isThrowing) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if(forceGrowing){
            throwForceTimer += Time.deltaTime; // Increment the throw force timer
        }else{
            throwForceTimer -= Time.deltaTime; // Decrement the throw force timer
        } // Increment the throw force timer
        if(throwForceTimer > maxThrowAccumulationTime)
        {
            forceGrowing = false; // Clamp the timer to the maximum accumulation time
        }
        if(throwForceTimer <= 0f){
            forceGrowing = true; // Reset the timer if it goes below zero
        }
        float forcePercentage = throwForceTimer / maxThrowAccumulationTime; // Calculate the force percentage based on the timer
        throwForceSquare.transform.localScale = new Vector3(originalForceScalex*(1+2.5f*forcePercentage), throwForceSquare.transform.localScale.y, throwForceSquare.transform.localScale.z); 

        // Vector2 direction = throwJoystick.Direction;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Debug.Log($"Throw angle: {angle}  ForceSquare:{throwForceSquare.transform.localScale.x} ThrowForceTimer:{throwForceTimer}");
        // playerCenter.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        
    }

    void HideThrowArrow(){
        if(mBomb==null){
            if (playerCenter != null)
            {       
                playerCenter.SetActive(false);
            }
        }
    }
    void DragFinishHandler(float duration){
        if (!isActive || mBomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        
        Vector2 force;
        // force.x = throwJoystick.lastNonZeroHorizontal * duration * 15f;
        // force.y = throwJoystick.lastNonZeroVertical * duration * 15f;

        force.x = throwJoystick.lastNonZeroHorizontal * throwForceTimer*2.5f * 10f;
        force.y = throwJoystick.lastNonZeroVertical * throwForceTimer*2.5f * 10f;

        Debug.Log($"DragFinished throw force:{force}  lastNonZeroVertical:{throwJoystick.lastNonZeroVertical}   lastNonZeroHorizontal:{throwJoystick.lastNonZeroHorizontal}");
        mBomb.transform.SetParent(null); // Detach from player
        mBomb.Throw(force,1f);
        mBomb = null;
        isThrowing = false;


        if (playerCenter != null)
        {
            playerCenter.SetActive(false);
        }
        // if(moveJoystickUI != null){
        //     moveJoystickUI.SetActive(false);
        // }
    }

    public void ThrowControlFinish(){
        if (!isActive || mBomb == null || !throwInitiated) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if(throwForceTimer>throwMinTime){
            Vector2 force;
            // force.x = throwJoystick.lastNonZeroHorizontal * duration * 15f;
            // force.y = throwJoystick.lastNonZeroVertical * duration * 15f;

            force.x = transform.localScale.x * throwForceTimer*2.5f * 10f;
            // force.y = throwJoystick.lastNonZeroVertical * throwForceTimer*2.5f * 10f;
            force.y = 0f;

        //  Debug.Log($"DragFinished throw force:{force}  lastNonZeroVertical:{throwJoystick.lastNonZeroVertical}   lastNonZeroHorizontal:{throwJoystick.lastNonZeroHorizontal}");
            mBomb.transform.SetParent(null); // Detach from player
            mBomb.Throw(force,1f);
            mBomb = null;
        }


        throwInitiated = false;
        isThrowing = false;
        if (playerCenter != null)
        {
            playerCenter.SetActive(false);
        }
    }
    void ThrowWithTouch(){
        if (!isActive || mBomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        if(!isTouchControl){
            return;
        }
        //float x = 0f;
        //float y = 0f;
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
    float throwForceTimer = 0f; // Timer for throw force accumulation
    [SerializeField] float maxThrowAccumulationTime = 0.7f; // Maximum time to accumulate throw force
    void InitiateThrow(){
        
        if (!isActive || mBomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        throwForceTimer = 0f; // Reset the throw force timer
        isThrowing = true; // Set the throwing flag to true
        if (playerCenter != null)
        {
            playerCenter.SetActive(true);
        }
    }
    public void InitiateThrow2(){
        if (!isActive || mBomb == null) // Only accumulate force if the player is active and has a bomb
        {
            return; // Exit if not active or no bomb
        }
        throwForceTimer = 0f; // Reset the throw force timer
        isThrowing = true; // Set the throwing flag to true
        throwInitiated = true;
        if (playerCenter != null)
        {
            playerCenter.SetActive(true);
        }
    }
    void AccumulateThrowForce(){
        // Check if the player has pressed the throw button (e.g., left mouse button or a specific key)
        if (!isActive || mBomb == null) // Only accumulate force if the player is active and has a bomb
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
            
            mBomb.transform.SetParent(null); // Detach from player

            
            // = GameObject.FindWithTag("TestLandingTransform").transform;
            Vector2 topLeft = new Vector2(-screenBounds.x, screenBounds.y);
            Vector2 force = throwForceVector2;
            force.x = transform.localScale.x * force.x; 
            // force.x = Mathf.Clamp(force.x, -screenBounds.x, screenBounds.x);
            // force.y = Mathf.Clamp(force.y, -screenBounds.y, screenBounds.y);
           // landingTarget.position = topLeft;
            // Vector2 landingPosition = landingTarget != null ? landingTarget.position : (Vector2)transform.position + throwDirection * 5f;
            mBomb.Throw(force, 1f);
            mBomb = null; // Reset bomb reference
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
    public void GameOver(){
        // isAlive = false;
        myAnimator.SetBool("isRunning", false);
        // myAnimator.SetBool("isJumping", false);
        myAnimator.Play("BetsyWin");
        // myAnimator.StartPlayback();
        // myRigidbody.velocity = Vector2.zero;
        // myRigidbody.isKinematic = true; // Stop all movement
        // myRigidbody.simulated = false; // Disable physics simulation
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
            StopMoving();
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
        moveInput.Normalize(); // Normalize the input vector to ensure consistent speed in all directions
    
    }
    public void StopMoving(){
        moveInput = Vector2.zero;
        myRigidbody.linearVelocity = Vector2.zero;
        myAnimator.SetBool("isRunning", false);
     //   Debug.Log($"StopMoving ===> isRightPlayer:{isRightSide} moveInput:{moveInput}");
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
        // if(isThrowing || !isAlive || !isActive){
        //     StopMoving();
        //     return;
        // }
        if(!isAlive || !isActive){
            StopMoving();
            return;
        }
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
        // if(isRightSide){
        //     clampedX = Mathf.Clamp(myRigidbody.position.x, playerHomeScript.homeTopLeft.x + playerHalfWidth, playerHomeScript.homeTopRight.x - playerHalfWidth);
        // }else{
        //     // clampedX = Mathf.Clamp(myRigidbody.position.x, -screenBounds.x + playerHalfWidth, 0 - playerHalfWidth);
        //     clampedX = Mathf.Clamp(myRigidbody.position.x, playerHomeScript.homeTopLeft.x + playerHalfWidth, playerHomeScript.homeTopRight.x - playerHalfWidth);
        // }
//        Debug.Log($"Clamping player position: x={myRigidbody.position.x} between {playerHomeScript.homeTopLeft.x + playerHalfWidth} and {playerHomeScript.homeTopRight.x - playerHalfWidth}");
        clampedX = Mathf.Clamp(myRigidbody.position.x, playerHomeScript.homeTopLeft.x + playerHalfWidth, playerHomeScript.homeTopRight.x - playerHalfWidth);
        // clampedY = Mathf.Clamp(myRigidbody.position.y, playerHomeScript.homeBottomLeft.y + playerHalfHeight, playerHomeScript.homeTopLeft.y - playerHalfHeight);
        clampedY = Mathf.Clamp(myRigidbody.position.y, playerHomeScript.homeBottomLeft.y + playerHalfHeight, playerHomeScript.homeTopLeft.y);
        Vector2 pos = transform.position;
        pos.x = clampedX;
        pos.y = clampedY;
        // if(!isRightSide)
        //     Debug.Log($"Player position adjusted to: {pos}");
        transform.position = pos;
    }

    void Run2(){
        if(isThrowing || !isAlive || !isActive){
            return;
        }
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
                if(mBomb!=null){
                    mBomb.FlipBombText(); // Flip the bomb text if the player is facing a different direction
                }
                //flip center
                // if(playerCenter != null){
                //     playerCenter.transform.localScale = new Vector3(-playerCenter.transform.localScale.x, playerCenter.transform.localScale.y, playerCenter.transform.localScale.z);
                // }
            }
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.linearVelocityX), 1f);

        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("OnTriggerEnter2D collision with: " + collision.tag);
        if(collision.tag == "Bomb" && mBomb==null){
            mBomb = collision.gameObject.GetComponent<Bomb>();
            //if(bomb != null && !bomb.GetComponent<Bomb>().hasOwner) { 
            if(mBomb != null && !mBomb.GetComponent<Bomb>().hasOwner && !mBomb.beingThrown) {  // Check if the bomb has no owner
                mBomb.SetHasOwner(true);
                mBomb.beingThrown = false;
                mBomb.OnBombExplode+=BombExplodeHandler;
                PickupBomb(mBomb);
                // if(moveJoystickUI != null){
                //     moveJoystickUI.SetActive(true);
                // }
                // bomb.AddTime(5f); // Optional: Add time to the bomb countdown
            }else{
                mBomb = null;
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
    void BombExplodeHandler(Vector2 position, Bomb bomb){
        mBomb=null;
    }
}
