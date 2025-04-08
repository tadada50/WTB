using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float Countdown = 10.0f;
    [SerializeField] GameObject explosionEffect; // Optional: Particle system for explosion effect
    [SerializeField] float bombTimerRevealTime = 3.0f; // Time the bomb timer is revealed to the player
    [SerializeField] public float bombExplosionRadius = 3.0f; // Radius of the explosion effect
    TMP_Text countdownText; // Optional: UI text to display countdown
    public bool hasOwner = false; // Flag to check if the bomb has an owner
    private Vector3 _velocity;
    public bool beingThrown = false; // Flag to check if the bomb is being thrown
    private Transform targetPosition;
    float _throwForce;
    Vector2 targetPosition2D;
    bool exploded = false;
    float timeRevealed = 0f; // Time when the bomb timer was revealed to the player
    Rigidbody2D bombBodyRb;
    public delegate void OnBombExplodeDelegate(Vector2 explodePosition, Bomb bomb);
    public event OnBombExplodeDelegate OnBombExplode;
    float halfBombWidth;
    float halfBombHeight;
    GameObject playground;
    BoxCollider2D playGroundCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Bounds playGroundBounds = playGroundCollider.bounds;

        // bombBodyRb.GetComponent<SpriteRenderer>().tag = "BombBody"; 
        halfBombWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        halfBombHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TMP_Text>();
        }
        Rigidbody2D[] rigidbody2Ds = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbody2Ds)
        {
            if (rb.CompareTag("BombBody"))
            {
                bombBodyRb = rb;
                break; // Exit the loop once we find the BombBody Rigidbody2D
            }
        }
        playground = GameObject.FindGameObjectsWithTag("Playground")[0];
        playGroundCollider = playground.GetComponent<BoxCollider2D>();
        SetBombCorners(playground);
        // bombBodyRb = GameObject.FindWithTag("BombBody").GetComponent<Rigidbody2D>()
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
            //FollowParaBolicPath2();

        }
        HideShadow();
        // GetComponent<SpriteRenderer>().sortingLayerName = "Bomb";
        if(bombBodyRb != null)
        {
            bombBodyRb.GetComponent<SpriteRenderer>().tag = "BombBody";
        }

        // FlipBombText();
  
    }
    void HideShadow(){
        SpriteRenderer shadowRenderer = GetComponent<SpriteRenderer>();
        if(!beingThrown || exploded){
            // Hide the shadow when the bomb is not being thrown
            if (shadowRenderer != null)
            {
                shadowRenderer.enabled = false; // Disable the shadow renderer
            }
        }else{
            if (shadowRenderer != null && !exploded)
            {
                shadowRenderer.enabled = true; // Disable the shadow renderer
            }
        }
    }
    void UpdateText(){
        bombTimerRevealTime -= Time.deltaTime; // Decrease the reveal time countdown
        if(bombTimerRevealTime<0){
            countdownText.text = string.Format("??:??:??");
            return;
        }
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
    void StopSparkles()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            // Stop the particle system
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
    private void Explode()
    {
        // Add explosion logic here
        if (countdownText != null)
        {
            countdownText.enabled = false;
        }
        // var renderers = GetComponents<SpriteRenderer>();
        // foreach (var renderer in renderers)
        // {
        //     renderer.enabled = false;
        // }
        HideShadow();
        StopSparkles();
        PlayExplosion();
        Vector2 explosionPosition = bombBodyRb.position;
       // explosionPosition.y -= bombBodyRb.GetComponent<SpriteRenderer>().bounds.size.y; // Use the bomb body position for explosion
        // explosionPosition.y -= bombBodyRb.GetComponent<SpriteRenderer>().bounds.size.y; // Use the bomb body position for explosion



        OnBombExplode(explosionPosition, this);
        // OnBombExplode(transform.position);
        Destroy(gameObject,2f);
        Destroy(bombBodyRb.gameObject);
    }
    public void FlipBombText(){
        // if(transform.localScale.x < 0){
        //     countdownText.transform.localScale = new Vector3(Mathf.Abs(countdownText.transform.localScale.x), countdownText.transform.localScale.y, countdownText.transform.localScale.z);
        // }
            // Flip the text if the parent transform is flipped
            // countdownText.transform.localScale = new Vector3(Mathf.Abs(countdownText.transform.localScale.x), countdownText.transform.localScale.y, countdownText.transform.localScale.z);
        if(countdownText!=null)
            countdownText.transform.localScale = new Vector3(countdownText.transform.localScale.x * -1, countdownText.transform.localScale.y, countdownText.transform.localScale.z);
    }
    public void SetTimer(float time)
    {
        // Set the countdown to a specific time
        Countdown = time;
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
    public void Throw2(Transform worldPosition, float throwForce)
    {
        beingThrown = true;
        targetPosition = worldPosition;
        _throwForce = throwForce; // Set the throw force for the bomb;
        hasOwner = false;
        // Get the rigidbody component
    }
    private bool isThrowingDown =false;
    private void SetIsThrowingDown(){
        if(targetPosition2D.y < transform.position.y){
            isThrowingDown = true;
        }else{
            isThrowingDown = false;
        }
    }
    float totalDistanceToDestination;
    Vector2 playBounds;
    void CaclulatePlayBounds(){
        playBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playBounds = new Vector2( playGroundCollider.bounds.extents.x, playGroundCollider.bounds.extents.y);
        // Convert playground extents to positive dimensions
    }
    
    Vector2 bombBoundTopRight;
    Vector2 bombBoundTopLeft;
    Vector2 bombBoundBottomRight;
    Vector2 bombBoundBottomLeft;

    private void SetBombCorners (GameObject go)
    {
        float width = go.GetComponent<BoxCollider2D> ().bounds.size.x;
        float height = go.GetComponent<BoxCollider2D> ().bounds.size.y;

        Vector2 topRight = go.transform.position, topLeft = go.transform.position, bottomRight = go.transform.position, bottomLeft = go.transform.position;

        topRight.x += width / 2;
        topRight.y += height / 2;

        topLeft.x -= width / 2;
        topLeft.y += height / 2;

        bottomRight.x += width / 2;
        bottomRight.y -= height / 2;

        bottomLeft.x -= width / 2;
        bottomLeft.y -= height / 2;
        
        bombBoundTopRight = new Vector2(topRight.x, topRight.y);
        bombBoundTopLeft = new Vector2(topLeft.x, topLeft.y);
        bombBoundBottomRight = new Vector2(bottomRight.x, bottomRight.y);
        bombBoundBottomLeft = new Vector2(bottomLeft.x, bottomLeft.y);
        // Draw rectangle in gizmos based on corners

        
    }
    public void Throw(Vector2 force, float throwForce)
    {
        // Debug.Log($"Throwing bomb to position: {force}");
        beingThrown = true;
        targetPosition2D = (Vector2)transform.position + force;
        playBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Debug.Log($"Play Bounds: {playBounds}");
        playBounds = new Vector2( playGroundCollider.bounds.extents.x, playGroundCollider.bounds.extents.y);
        //playBounds = new Vector2(playGroundCollider.bounds.size.x, playGroundCollider.bounds.size.y);
        // Convert playground extents to positive dimensions
        
        Debug.Log($"Play Bounds: {playBounds}");
        targetPosition2D = new Vector2(
            // Mathf.Clamp(targetPosition2D.x, -playBounds.x + 2*halfBombWidth, playBounds.x - 2*halfBombWidth),
            // Mathf.Clamp(targetPosition2D.y, -playBounds.y + 2*halfBombHeight, playBounds.y - 2*halfBombHeight)
            Mathf.Clamp(targetPosition2D.x, bombBoundBottomLeft.x+ 2*halfBombWidth, bombBoundBottomRight.x - 2*halfBombWidth),
            Mathf.Clamp(targetPosition2D.y, bombBoundBottomLeft.y + 8*halfBombHeight, bombBoundTopLeft.y - 2*halfBombHeight)
        );
        
        totalDistanceToDestination = Vector2.Distance(transform.position, targetPosition2D);
        Debug.Log($"Target Position: {targetPosition2D} Total Distance: {totalDistanceToDestination}");
        // initialBombPosition = transform.position; // Store the initial position of the bomb
        SetIsThrowingDown();
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
    [SerializeField] public float bombMaxHeight= 6f;
    float flySpeed = 10f;

private void FollowStraightPath(){ 
    if (bombBodyRb == null)
    {
        return;
    }
    if (Vector2.Distance(transform.position, targetPosition2D) < 0.01f)
    {
        transform.position = targetPosition2D; // Snap to the target position
        bombBodyRb.transform.localPosition = new Vector2(0,0);
        beingThrown = false;
        return;
    }

    distanceToDestination = Vector2.Distance(transform.position, targetPosition2D);
    float progress = 1 - (distanceToDestination / totalDistanceToDestination);
    
    // Adjust speed based on total distance
    float baseSpeed = Mathf.Clamp(totalDistanceToDestination, 0f, 20f);
    float speedMultiplier = 1f * Mathf.Cos(progress * Mathf.PI * 0.5f);
    speedMultiplier = Mathf.Clamp(speedMultiplier, 0.5f, 1f);
    
    // if(totalDistanceToDestination < 1f){
    //     baseSpeed = totalDistanceToDestination;;
    // }
    
    float delta = baseSpeed * speedMultiplier * Time.deltaTime;
    float newHeight = -4 * bombMaxHeight * progress * (progress - 1);
    
    transform.position = Vector2.MoveTowards(transform.position, targetPosition2D, delta);
    bombBodyRb.transform.localPosition = new Vector2(0, newHeight);
}
    Vector2 initialBombPosition;
    Vector2 totalDistance;
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, targetPosition2D);
        // Gizmos.DrawSphere(targetPosition2D, 0.1f); // Draw a sphere at the target position
        // // Gizmos.color = Color.green;
        // // Gizmos.DrawWireCube(playground.transform.position, playBounds);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(bombBoundTopLeft, bombBoundTopRight);
        // Gizmos.DrawLine(bombBoundTopRight, bombBoundBottomRight);  
        // Gizmos.DrawLine(bombBoundBottomRight, bombBoundBottomLeft);
        // Gizmos.DrawLine(bombBoundBottomLeft, bombBoundTopLeft);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombExplosionRadius);

    }

    void PlayExplosion(){
        if(explosionEffect != null){
           // GameObject instance = Instantiate(explosionEffect,transform.position,Quaternion.identity,transform);
            GameObject instance = Instantiate(explosionEffect,bombBodyRb.position,Quaternion.identity,transform);
            Destroy(instance.gameObject, 2f);
        }
    }

    
}
