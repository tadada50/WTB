using UnityEngine;

public class GadgetBehavior : MonoBehaviour
{
    [SerializeField] Sprite activatedSprite;
    [SerializeField] Sprite neutralSprite;

    [SerializeField] float bombTimeModifier = 5.0f;
    [SerializeField] LevelManager levelManager;
    [SerializeField] int activationLimit = 1;
    int activationTimes = 0;

    bool isPressed = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGadgetState();
    }

    private string[] validTags = { "RightPlayer", "LeftPlayer", "Bomb" };  // Add or modify tags as needed
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
        {
            isPressed = true;
            GetComponent<SpriteRenderer>().sprite = activatedSprite;
            ActivateGadget();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
        {
            isPressed = false;
            GetComponent<SpriteRenderer>().sprite = neutralSprite;
        }
    }

    public void ActivateGadget()
    {
        activationTimes++;
        if(activationTimes>activationLimit){
            return;
        }
        levelManager.AddTimeToBomb(bombTimeModifier);
    }
    void UpdateGadgetState()
    {
        if (activationTimes >= activationLimit)
        {
            Destroy(gameObject,0.5f);
        }

    }
    // public void ActivateGadget()
    // {
    //     GetComponent<SpriteRenderer>().sprite = activatedSprite;
    // }

}