using UnityEngine;

public class GadgetBehavior : MonoBehaviour
{
    [SerializeField] protected Sprite activatedSprite;
    [SerializeField] protected Sprite neutralSprite;

    [SerializeField] protected float bombTimeModifier = 5.0f;
    [SerializeField] protected LevelManager levelManager;
    [SerializeField] protected int activationLimit = 1;
    protected int activationTimes = 0;

    protected bool isActive = true;

    //bool isPressed = false;

    void Start()
    {
        if(bombTimeModifier < -50){
            bombTimeModifier = Random.Range(-10, 11);
        }
        if (levelManager == null)
        {
            levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGadgetState();
    }

    protected string[] validTags = { "RightPlayer", "LeftPlayer", "Bomb" };  // Add or modify tags as needed
    
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("==>OnTriggerEnter2D: " + collision.gameObject.name);
        if(isActive == false){
            return;
        }
        if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
        {
             Debug.Log("Gadget activated by: " + collision.gameObject.name);
            //  isPressed = true;
            //  GetComponent<SpriteRenderer>().sprite = activatedSprite;
            //  bool isRightSide = collision.gameObject.CompareTag("RightPlayer");
            //  ActivateGadgetFunction(isRightSide);
          //  isPressed = true;
            GetComponent<SpriteRenderer>().sprite = activatedSprite;
            bool isRightSide = collision.gameObject.CompareTag("RightPlayer");
            ActivateGadgetFunction(isRightSide);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(isActive == false){
            return;
        }
        if (System.Array.Exists(validTags, tag => collision.gameObject.CompareTag(tag)))
        {
          //  isPressed = false;
            GetComponent<SpriteRenderer>().sprite = neutralSprite;
        }
    }

    public virtual void ActivateGadgetFunction(bool isRightSide)
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
            isActive = false;
            GetComponent<SpriteRenderer>().sprite = neutralSprite;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

        }

    }
    public void ActivateGadget()
    {
        // Debug.Log("Activating gadget");
        GetComponent<SpriteRenderer>().sprite = neutralSprite;
        isActive = true;
        activationTimes = 0;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

}