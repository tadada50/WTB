using TMPro;
using UnityEngine;

public enum alphaValue{
    GROWING,
    SHRINKING
}

public class BlinkingText : MonoBehaviour
{
    TMP_Text countdownText; 
    public alphaValue currentAlphaValue = alphaValue.GROWING; // Default to growing at start
    public float CommentMinAlpha;
    public float CommentMaxAlpha;
    public float CommentCurrentAlpha;
    void Start()
    {
        CommentMinAlpha = 0.2f;
        CommentMaxAlpha = 1.0f;
        CommentCurrentAlpha = 1f;
        currentAlphaValue = alphaValue.GROWING;
        if (countdownText == null)
        {
            countdownText = GetComponent<TMP_Text>();
        }
        
    }
    void Update()
    {
        AlphaComments();
    }

    public void AlphaComments(){
        
        // Check the current alpha value and adjust accordingly
        if (currentAlphaValue == alphaValue.GROWING)
        {
            CommentCurrentAlpha += Time.deltaTime * 1.7f; // Adjust speed as needed
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, CommentCurrentAlpha); // Update the text color with the new alpha value
            if (CommentCurrentAlpha >= CommentMaxAlpha)
            {
                CommentCurrentAlpha = CommentMaxAlpha;
                currentAlphaValue = alphaValue.SHRINKING; // Switch to shrinking when max is reached
            }
        }
        else if (currentAlphaValue == alphaValue.SHRINKING)
        {
            CommentCurrentAlpha -= Time.deltaTime * 1.4f; // Adjust speed as needed
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, CommentCurrentAlpha); 
            if (CommentCurrentAlpha <= CommentMinAlpha)
            {
                CommentCurrentAlpha = CommentMinAlpha;
                currentAlphaValue = alphaValue.GROWING; // Switch to growing when min is reached
            }
        }
    }
}
