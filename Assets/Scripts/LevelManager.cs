using System.Collections;
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
            StartCoroutine(LoadNextBomb(1f));
            

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
        }else if(currentGameState == GameState.GameOver)
        {
            // Handle game over logic here
            // For example, you might want to reset the game or show a game over screen
           // Debug.Log("Game Over!"); // Placeholder for game over logic
            foreach (var player in players)
            {
                // player.GetComponent<PlayerMovement>().SetActive(true);
                player.GetComponentInChildren<PlayerMovement>().SetActive(false);
            }
            currentGameState = GameState.BombDropping;
        }
    }
    void InitBomb(){
            Debug.Log("==>InitBomb");
            GameObject bombInstance = Instantiate(bombPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Bomb bomb = bombInstance.GetComponent<Bomb>();
            // bomb.AddTime(1000);
            if (bomb != null)
            {
                bomb.OnBombExplode+=BombExplodeHandler;
                bombs.Add(bomb);
            }
            bombInstance.GetComponent<SpriteRenderer>().sortingLayerName = "Players";
            bombInstance.tag = "Bomb";
            bombInstance.GetComponent<Renderer>().enabled = true;
    }
    void BombExplodeHandler(Vector2 position){
        // Handle bomb explosion here
        // Determine which side of the screen the explosion occurred
        int quadrant = 0;;
        if(position.x == 0 && position.y == 0){
            // If the explosion is exactly at the center, you can decide how to handle it
            quadrant = 0; // Center
        }else if(position.x > 0 && position.y > 0){
            // If the explosion is exactly on the vertical axis
            quadrant = 1; // Top Right Quadrant
        }else if(position.x < 0 && position.y > 0){
            quadrant = 2; 
        }else if(position.x < 0 && position.y < 0){
            quadrant = 3; 
        }else if(position.x > 0 && position.y < 0){
            quadrant = 4; 
        }
        // string horizontalSide = position.x < 0 ? "left" : "right";
        // if (position.x == 0){
        //     // If the explosion is exactly at the center, you can decide how to handle it
        //     horizontalSide = "Middle";
        // }
        // string verticalSide = position.y < 0 ? "bottom" : "top";
        // if (position.y == 0){
        //     // If the explosion is exactly at the center, you can decide how to handle it
        //     horizontalSide = "Middle";
        // }
        Debug.Log($"Bomb exploded in quadrant {quadrant}  at position {position}");
        currentGameState = GameState.GameOver;
    }
    IEnumerator LoadNextBomb(float secondsDelay){

        Debug.Log("==>LoadNextBoomb");
        yield return new WaitForSeconds(secondsDelay);    
        InitBomb();    
    }
}
