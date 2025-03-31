using UnityEngine;

public class SwipeControls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
            }
            else if (touch.phase == TouchPhase.Moved) // Check if the touch is moving
            {
                // Handle touch move logic here, e.g., calculate swipe direction
                Debug.Log("Touch moved to: " + touch.position);
            }
            else if (touch.phase == TouchPhase.Ended) // Check if the touch has ended
            {
                // Handle touch end logic here, e.g., determine swipe direction
                Debug.Log("Touch ended at: " + touch.position);
            }

            i++;
        }
    }
}
