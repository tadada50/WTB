using UnityEngine;

public class WalkingBomb : Bomb
{
    [SerializeField] public GameObject leftPlayerHome;
    [SerializeField] public GameObject rightPlayerHome;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("WalkingBomb initialized.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
