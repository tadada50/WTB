using UnityEngine;

public class SwipeControls : MonoBehaviour
{
    Vector2 screenBounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        // Print the screen bounds corners in world coordinates
        Vector2 topRight = new Vector2(screenBounds.x, screenBounds.y);
        Vector2 topLeft = new Vector2(-screenBounds.x, screenBounds.y);
        Vector2 bottomRight = new Vector2(screenBounds.x, -screenBounds.y);
        Vector2 bottomLeft = new Vector2(-screenBounds.x, -screenBounds.y);
    }

    Touch leftPlayerTouch;
    Touch rightPlayerTouch;
    [SerializeField] float swipeTimeThreshold = 0.2f; // Time threshold for swipe detection
    [SerializeField] float swipeDistanceThreshold = 50f; // Distance threshold for swipe detection
    [SerializeField] PlayerMovement leftPlayerMovement; // Reference to the left player movement script (if needed)
    [SerializeField] PlayerMovement rightPlayerMovement; // Reference to the right player movement script (if needed)
    float rightPlayerSwipeTime; // Time when the right player swipe started
    float leftPlayerSwipeTime; // Time when the left player swipe started

    Vector2 rightPlayerMoveDestination; // Destination for the right player movement based on swipe
    Vector2 leftPlayerMoveDestination; // Destination for the left player movement based on swipe
    // Update is called once per frame
    void Update()
    {
        int i = 0;
        while(i < Input.touchCount) // Loop through all touches on the screen
        {
            Touch touch = Input.GetTouch(i); // Get the touch at index i

            if (touch.phase == TouchPhase.Began) // Check if the touch just began
            {
                // Handle touch start logic here, e.g., record the starting position
                Debug.Log("Touch started at: " + touch.position);
                if (touch.position.x < Screen.width / 2) // Check if touch is on the left side of the screen
                {
                    leftPlayerTouch = touch; // Assign to left player touch
                    leftPlayerSwipeTime = 0;
                    //move this to the left player touch logic if needed
                    leftPlayerMovement.RunWithTouch(touch.position); // Call a method on the left player movement script if needed
                    Debug.Log("Left player touch detected.");
                }
                else // Otherwise, it's on the right side
                {
                    rightPlayerTouch = touch; // Assign to right player touch
                    rightPlayerSwipeTime = 0;
                    rightPlayerMovement.RunWithTouch(touch.position); // Call a method on the right player movement script if needed
                    Debug.Log("Right player touch detected.");
                }
            }
            else if (touch.phase == TouchPhase.Moved) // Check if the touch is moving
            {
                if (touch.position.x < Screen.width / 2) // Check if touch is on the left side of the screen
                {
                    leftPlayerTouch = touch; // Assign to left player touch
                    leftPlayerSwipeTime += Time.deltaTime;
                    if (leftPlayerSwipeTime > swipeTimeThreshold){ //not a swipe
                            //move player 
                        leftPlayerMovement.RunWithTouch(touch.position);
                    }
                    Debug.Log("Left player touch detected.");
                }
                else // Otherwise, it's on the right side
                {
                    rightPlayerTouch = touch; // Assign to right player touch
                    rightPlayerSwipeTime += Time.deltaTime; // Increment the swipe time for the right player
                    if (rightPlayerSwipeTime > swipeTimeThreshold){ //not a swipe
                            //move player
                            rightPlayerMovement.RunWithTouch(touch.position); 
                    }
                    Debug.Log("Right player touch detected.");
                }

                Debug.Log("Touch moved to: " + touch.position);
            }
            else if (touch.phase == TouchPhase.Ended) // Check if the touch has ended
            {
                if (touch.position.x < Screen.width / 2) // Check if touch is on the left side of the screen
                {
                    leftPlayerSwipeTime += Time.deltaTime;
                    if (leftPlayerSwipeTime < swipeTimeThreshold){ //swipe detected
                            Vector2 swipeDistance = touch.position - leftPlayerTouch.position;
                            if (swipeDistance.magnitude > swipeDistanceThreshold)
                            {
                                // Swipe detected, calculate destination
                                Vector2 swipeDirection = swipeDistance.normalized;
                                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                                // leftPlayerMoveDestination = worldPosition;
                                Vector2 throwForce = swipeDirection * swipeDistance.magnitude * 2f;
                                // You'll need to apply this force to your bomb object using Rigidbody2D.AddForce or similar
                                leftPlayerMovement.ThrowBombWithTouch(throwForce);
                                Debug.Log("Left player swipe detected: " + swipeDirection);
                            }
                    }
                    Debug.Log("Left player touch detected.");
                }
                else // Otherwise, it's on the right side
                {
                    rightPlayerSwipeTime += Time.deltaTime;
                    if (rightPlayerSwipeTime < swipeTimeThreshold){ //swipe detected
                            Vector2 swipeDistance = touch.position - leftPlayerTouch.position;
                            if (swipeDistance.magnitude > swipeDistanceThreshold)
                            {
                                // Swipe detected, calculate destination
                                Vector2 swipeDirection = swipeDistance.normalized;
                                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                                // leftPlayerMoveDestination = worldPosition;
                                Vector2 throwForce = swipeDirection * swipeDistance.magnitude * 2f;
                                rightPlayerMovement.ThrowBombWithTouch(throwForce); // Call a method on the right player movement script to throw the bomb
                                // You'll need to apply this force to your bomb object using Rigidbody2D.AddForce or similar
                                Debug.Log("Right player swipe detected: " + swipeDirection);
                            }
                    }

                    Debug.Log("Right player touch detected.");
                }
                Debug.Log("Touch ended at: " + touch.position);
            }

            i++;
        }
    }
}
