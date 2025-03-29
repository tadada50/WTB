using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    BombDropping,
    PlayerActive,
}
public class LevelManager : MonoBehaviour
{
    [SerializeField] GameState currentGameState = GameState.BombDropping;
    [SerializeField] List<GameObject> players;
    [SerializeField] int bombNumber = 1; // Number of bombs to drop
    [SerializeField] float bombDropInterval = 2.0f; // Interval between bomb drops
    [SerializeField] GameObject bombPrefab; // Prefab for the bomb
    List<Bomb> bombs = new List<Bomb>(); // Assuming you have a Bomb class to manage bomb logic
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGameState == GameState.BombDropping)
        {   
            //instantiate bomb
            InitBomb();
            //drop bomb
            // Countdown
            currentGameState = GameState.PlayerActive; // Change game state to PlayerActive after dropping bombs
        }
        else if(currentGameState == GameState.PlayerActive)
        {
            // Handle player active logic here
            foreach (var player in players)
            {
                // player.GetComponent<PlayerMovement>().SetActive(true);
                player.GetComponentInChildren<PlayerMovement>().SetActive(true);
            }
        }
    }
    void InitBomb(){
            GameObject bombInstance = Instantiate(bombPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Bomb bomb = bombInstance.GetComponent<Bomb>();
            bomb.AddTime(1000);
            if (bomb != null)
            {
                bombs.Add(bomb);
            }
            bombInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Players";
            bombInstance.tag = "Bomb";
            bombInstance.GetComponent<Renderer>().enabled = true;
    }
}
