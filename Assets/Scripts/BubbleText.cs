using TMPro;
using UnityEngine;

public class BubbleText : MonoBehaviour
{
    // [SerializeField] bool isFloating = false;
    [SerializeField] float floatingSpeed = 2f; // Speed of floating effect
    TMP_Text displayText; 
    public alphaValue currentAlphaValue = alphaValue.GROWING; // Default to growing at start
    public float CommentMinAlpha;
    public float CommentMaxAlpha;
    public float CommentCurrentAlpha;

    public float CommentMaxScale = 2f;
    private bool isPlaying = false;

    private Vector3 originalPosition;
    void Start()
    {
        // originalPosition = gameObject.GetComponent<RectTransform>().position;
        // if (originalPosition == null)
        // {
        //     originalPosition = gameObject.GetComponent<Transform>() as RectTransform;
        // }
        if (displayText == null)
        {
            displayText = GetComponent<TMP_Text>();
        }
        // Debug.Log($"Original position: {originalPosition}");
        ResetValues();        
    }
    void ResetValues(){
        CommentMinAlpha = 0.2f;
        CommentMaxAlpha = 1.0f;
        CommentCurrentAlpha = 0f;
        currentAlphaValue = alphaValue.GROWING;
        isPlaying = false;

        // transform.localScale = new Vector3(1f, 1f, 1f); // Reset scale to normal size
        // transform.position = originalPosition.position; // Reset position to original
      
        // gameObject.GetComponent<RectTransform>().position = originalPosition; // Reset position to original
        float scaleX = (gameObject.GetComponent<RectTransform>().localScale.x >= 0) ? 1f : -1f;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(scaleX, 1f, 1f); // Reset scale to normal size
        // Debug.Log($"==>ResetValue Original position: {originalPosition}  GameObjectPosition: {gameObject.GetComponent<RectTransform>().position}");
        displayText.enabled = false;
    }
    void Update()
    {
        if(isPlaying == false){
            return;
        }
        AlphaComments();
    }

    public void InitTextDisplay(){
        ResetValues(); // Reset values before starting the display
        isPlaying = true; // Start the display
        displayText.enabled = true;
    }
    public void AlphaComments(){
        
        RectTransform gameRectTransform = gameObject.GetComponent<RectTransform>();
        // Check the current alpha value and adjust accordingly
        if (currentAlphaValue == alphaValue.GROWING)
        {
            CommentCurrentAlpha += Time.deltaTime * 1.2f; // Adjust speed as needed
            displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, CommentCurrentAlpha); // Update the text color with the new alpha value
            float scaleY = transform.localScale.y * (1 + Time.deltaTime * 1f); // Adjust speed as needed
            float scaleX = transform.localScale.x * (1 + Time.deltaTime * 1f);
            // if(gameRectTransform.localScale.y < 0){
            //     scaleX = -scale;
            // }

            //transform.localScale = new Vector3(scale, scale, 1f); // Scale the text based on alpha value            
            // transform.position = new Vector3(originalPosition.position.x, originalPosition.position.y + Mathf.Sin(Time.time * 2) * 0.1f, originalPosition.position.z); // Floating effect
            //transform.position = new Vector3(originalPosition.position.x, transform.position.y + Time.deltaTime*floatingSpeed, originalPosition.position.z); // Floating effect


            gameRectTransform.localScale = new Vector3(scaleX, scaleY, 1f);
            // gameRectTransform.position = new Vector3(originalPosition.x, gameRectTransform.position.y + Time.deltaTime*floatingSpeed, originalPosition.z); // Floating effect


            if (CommentCurrentAlpha >= CommentMaxAlpha)
            {
                CommentCurrentAlpha = CommentMaxAlpha;
                currentAlphaValue = alphaValue.SHRINKING; // Switch to shrinking when max is reached
            }
        }else{
            ResetValues();
        }
        // else if (currentAlphaValue == alphaValue.SHRINKING)
        // {
        //     CommentCurrentAlpha -= Time.deltaTime * 1.4f; // Adjust speed as needed
        //     displayText.color = new Color(displayText.color.r, displayText.color.g, displayText.color.b, CommentCurrentAlpha); 
        //     if (CommentCurrentAlpha <= CommentMinAlpha)
        //     {
        //         CommentCurrentAlpha = CommentMinAlpha;
        //         currentAlphaValue = alphaValue.GROWING; // Switch to growing when min is reached
        //     }
        // }
    }
}
