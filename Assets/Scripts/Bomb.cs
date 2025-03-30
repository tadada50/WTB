using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float Countdown = 10.0f;
    [SerializeField] GameObject explosionEffect; // Optional: Particle system for explosion effect
    TMP_Text countdownText; // Optional: UI text to display countdown
    public bool hasOwner = false; // Flag to check if the bomb has an owner
    private Vector3 _velocity;
    public bool beingThrown = false; // Flag to check if the bomb is being thrown
    private Transform targetPosition;
    float _throwForce;
    Vector2 targetPosition2D;
    bool exploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TMP_Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Countdown -= Time.deltaTime;
        UpdateText();
        if (Countdown <= 0 && !exploded)
        {
            exploded = true; // Set exploded to true to prevent multiple explosions
            Explode();
        }
        // Optional: Update velocity and position if you want to simulate physics manually
        if(beingThrown){
            FollowStraightPath();
        }
  
    }
    void UpdateText(){
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(Countdown / 60F);
            int seconds = Mathf.FloorToInt(Countdown - minutes * 60);  
            int milliseconds = Mathf.FloorToInt((Countdown - minutes * 60 - seconds) * 1000);
            // Format the countdown text as MM:SS:MS
            countdownText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
           // countdownText.text = Countdown.ToString("F1"); // Update countdown text
        }
    }

    private void Explode()
    {
        // Add explosion logic here
        if (countdownText != null)
        {
            countdownText.enabled = false;
        }
        var renderers = GetComponents<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        
        PlayExplosion();
        Destroy(gameObject, 2.5f);
    }
    public void AddTime(float timeToAdd)
    {
        Countdown += timeToAdd;
    }
    public void SetHasOwner(bool hasOwner)
    {
        this.hasOwner = hasOwner;
    }
    public float GetCountdown()
    {
        return Countdown;
    }
    [SerializeField] float timeToDestination = 1.0f; // Time it takes to reach the target position
    float distanceToDestination; // Distance to the target position
    float flyTime;
    public void Throw(Transform worldPosition, float throwForce)
    {
        beingThrown = true;
        targetPosition = worldPosition;
        _throwForce = throwForce; // Set the throw force for the bomb;
        hasOwner = false;
        // Get the rigidbody component
    }
    public void Throw(Vector2 force, float throwForce)
    {
        // Debug.Log($"Throwing bomb to position: {force}");
        beingThrown = true;
        targetPosition2D = (Vector2)transform.position + force;
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        targetPosition2D = new Vector2(
            Mathf.Clamp(targetPosition2D.x, -screenBounds.x, screenBounds.x),
            Mathf.Clamp(targetPosition2D.y, -screenBounds.y, screenBounds.y)
        );
        // targetPosition2D = force;
        _throwForce = throwForce; // Set the throw force for the bomb;
        hasOwner = false;
      //  timeToDestination = 1.0f; // Reset the time to destination if needed
        flyTime = 0;
        start = transform.position; // Store the starting position for the parabolic path calculation
        // Get the rigidbody component
    }
    private void UpdateVelocity()
    {
        _velocity += Physics.gravity * Time.deltaTime;
    }

    private void UpdatePosition()
    {
        transform.position += _velocity * Time.deltaTime;
    }

    Vector2 start;
    float maxHeight;
    float height= 2f;
    float flySpeed = 10f;
    //need to fix
    private void FollowParabolicPath(){
        if (Vector2.Distance(transform.position, targetPosition2D) < 0.1f)
        {
            beingThrown = false;
            return;
        }

        distanceToDestination = Vector2.Distance(transform.position, targetPosition2D);
        
        float totalDistance = Vector2.Distance(targetPosition2D, transform.position);
        float progress = 1 - (distanceToDestination / totalDistance);


        flyTime = flyTime + Time.deltaTime;
        height = 2f; // Maximum height of the arc, adjust as needed
        maxHeight = Mathf.Max(start.y, targetPosition2D.y) + height;
        float horizontalDistance = targetPosition2D.x - start.x;
        timeToDestination = horizontalDistance/flySpeed;

        float timeProgress = flyTime / timeToDestination; // Calculate progress based on flyTime and timeToDestination

        // float time = flyTime / timeToDestination;
        float a = (start.y - 2 * maxHeight + targetPosition2D.y) / (horizontalDistance * horizontalDistance);
        float b = (4 * maxHeight - 2 * start.y - 2 * targetPosition2D.y) / horizontalDistance;
        float c = start.y;
        // Using quadratic formula to find x: ax^2 + bx + (c - targetPosition2D.y) = 0
        float discriminant = b * b - 4 * a * (c - targetPosition2D.y);
        float xSolution = 0; // Default value for x if no solution is found
        if (discriminant >= 0)
        {
            float x1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            // Choose the x value closest to targetPosition2D.x
            xSolution = Mathf.Abs(x1 - targetPosition2D.x) < Mathf.Abs(x2 - targetPosition2D.x) ? x1 : x2;
        }
        else
        {
            // If no real solution, fallback to linear interpolation (straight line)
            xSolution = Mathf.Lerp(start.x, targetPosition2D.x, timeProgress);
        }
        float t = timeToDestination;
        
        

        

        // Calculate the y position using the quadratic formula
        // y = ax^2 + bx + c
        // Calculate the current position along the path
        // Vector2 start = transform.position;
       // float height = 2f; // Maximum height of the arc
        
        
        // Parabolic interpolation
        float x = Mathf.Lerp(start.x, targetPosition2D.x, timeProgress);
        //Debug.Log($"Y={newY} X={x} at time:{timeToDestination} = {xSolution}, start.y: {start.y}, targetPosition2D: {targetPosition2D}, maxHeight: {maxHeight}");

        // float y = start.y + height * (4 * flyTime - 4 * flyTime * flyTime);
        //float y = start.y + height * (4 * x - 4 * x * x);
        float newY = a*x*x + b*x + c-targetPosition2D.y; // Calculate the y position using the quadratic formula

        float speedMultiplier = Mathf.Cos(progress * Mathf.PI * 0.5f);
        float delta = (12f + distanceToDestination * 2f) * speedMultiplier * Time.deltaTime;
        // Move towards the calculated position
        Debug.Log($"Moving to ({x}, {newY}), target: {targetPosition2D}, from- transform.position: {transform.position} ,progress: {progress}, distanceToDestination: {distanceToDestination}, delta: {delta}");
        // transform.position = Vector2.MoveTowards(
        //     transform.position, 
        //     new Vector2(x, y), 
        //     delta
        // );
        transform.position = new Vector2(x, newY); // Set the position directly to the calculated point
        
    }
    private void FollowStraightPath(){
        if (Vector2.Distance(transform.position, targetPosition2D) < 0.1f)
        {
            beingThrown = false;
            return; // Stop following the path if close to the target
        }
        distanceToDestination = Vector2.Distance(transform.position, targetPosition2D);
        float progress = 1 - (distanceToDestination / Vector2.Distance(targetPosition2D, transform.position));
        float speedMultiplier = 1.5f*Mathf.Cos(progress * Mathf.PI * 0.5f);
        float delta = (12f + distanceToDestination * 2f) * speedMultiplier * Time.deltaTime;
       // Debug.Log($"Moving to ({x}, {y}), target: {targetPosition2D}, progress: {progress}, distanceToDestination: {distanceToDestination}, delta: {delta}");
        transform.position = Vector2.MoveTowards(transform.position, targetPosition2D, delta);
    }
    // private void FollowPath(){
    //     if (Vector2.Distance(transform.position, targetPosition.position) < 0.1f)
    //     {
    //         beingThrown = false;
    //     }
    //     float delta = _throwForce * Time.deltaTime;
    //     transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, delta);
    // }

    void PlayExplosion(){
        if(explosionEffect != null){
            GameObject instance = Instantiate(explosionEffect,transform.position,Quaternion.identity,transform);
            Destroy(instance.gameObject, 2f);
        }
    }

    
}
