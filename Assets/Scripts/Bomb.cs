using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float Countdown = 10.0f;
    TMP_Text countdownText; // Optional: UI text to display countdown
    public bool hasOwner = false; // Flag to check if the bomb has an owner
    private Vector3 _velocity;
    public bool beingThrown = false; // Flag to check if the bomb is being thrown
    private Transform targetPosition;
    float _throwForce;
    Vector2 targetPosition2D;

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
        if (Countdown <= 0)
        {
            Explode();
        }
        // Optional: Update velocity and position if you want to simulate physics manually
        if(beingThrown){
            FollowPath();
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

    // private void OnEnable()
    // {
    //     if (countdownText != null)
    //     {
    //         countdownText.text = Countdown.ToString("F1"); // Display initial countdown
    //     }
    // }
    private void Explode()
    {
        // Add explosion logic here
        Destroy(gameObject);
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
    private void FollowPath(){
        if (Vector2.Distance(transform.position, targetPosition2D) < 0.1f)
        {
            beingThrown = false;
        }
        float delta = 10f * Time.deltaTime;
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

    
}
