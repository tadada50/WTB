using UnityEngine;
using TMPro;

public class Bomb : MonoBehaviour
{
    [SerializeField] float Countdown = 10.0f;
    TMP_Text countdownText; // Optional: UI text to display countdown
    public bool hasOwner = false; // Flag to check if the bomb has an owner
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (countdownText == null)
        {
            countdownText = GetComponentInChildren<TMP_Text>();
            countdownText.material.mainTexture.filterMode = FilterMode.Point;
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
    private void OnEnable()
    {
        if (countdownText != null)
        {
            countdownText.text = Countdown.ToString("F1"); // Display initial countdown
        }
    }
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
}
