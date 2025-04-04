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
    [SerializeField] GameObject craterPrefab; // Prefab for the crater
    float healthyAreaLeft;
    float healthyAreaRight;
    List<GameObject> craters = new List<GameObject>();


    List<Bomb> bombs = new List<Bomb>(); // Assuming you have a Bomb class to manage bomb logic
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            foreach (var player in players)
            {
                // player.GetComponent<PlayerMovement>().SetActive(true);
                SetPlayerHomeCorners(player.GetComponentInChildren<PlayerMovement>());
            }
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
    void CalculateHealthyArea(){
        // Calculate the healthy area based on craters
        healthyAreaLeft = 0;
        healthyAreaRight = 0;

        foreach (var crater in craters) {
            Vector3 craterPosition = crater.transform.position;
            if (craterPosition.x < 0) {
                healthyAreaLeft += crater.GetComponent<Collider2D>().bounds.size.x;
            } else {
                healthyAreaRight += crater.GetComponent<Collider2D>().bounds.size.x;
            }
        }

    }
    void InitBomb(){
            // Debug.Log("==>InitBomb");
            GameObject bombInstance = Instantiate(bombPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Bomb bomb = bombInstance.GetComponent<Bomb>();
            bomb.SetTimer((float)Random.Range(20, 61));
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
    void BombExplodeHandler2(Vector2 position){
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
    void BombExplodeHandler(Vector2 position){
        // Handle bomb explosion here
        // Check which player's home the bomb exploded in
        foreach (var player in players) {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (position.x >= playerMovement.homeTopLeft.x && 
                position.x <= playerMovement.homeBottomRight.x &&
                position.y <= playerMovement.homeTopLeft.y && 
                position.y >= playerMovement.homeBottomRight.y) {
                Debug.Log($"Bomb exploded in {(playerMovement.isRightSide ? "Right" : "Left")} player's home!");
                GameObject craterInstance = Instantiate(craterPrefab, position, Quaternion.identity);
                craters.Add(craterInstance);
                CalculateHealthyArea(craterInstance);
            }
        }
        //Calculate the healthy area based on craters
        // Determine which side of the screen the explosion occurred
        // int quadrant = 0;;
        // if(position.x == 0 && position.y == 0){
        //     // If the explosion is exactly at the center, you can decide how to handle it
        //     quadrant = 0; // Center
        // }else if(position.x > 0 && position.y > 0){
        //     // If the explosion is exactly on the vertical axis
        //     quadrant = 1; // Top Right Quadrant
        // }else if(position.x < 0 && position.y > 0){
        //     quadrant = 2; 
        // }else if(position.x < 0 && position.y < 0){
        //     quadrant = 3; 
        // }else if(position.x > 0 && position.y < 0){
        //     quadrant = 4; 
        // }
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
        // Debug.Log($"Bomb exploded in quadrant {quadrant}  at position {position}");
        currentGameState = GameState.GameOver;
    }
    void CalculateHealthyArea(GameObject crater){
        SpriteRenderer spriteRenderer = crater.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) {
            return;
        }
        float width = spriteRenderer.bounds.size.x;
        float height = spriteRenderer.bounds.size.y;
    }
    void CalculateHealthyArea2(){
        // Calculate the healthy area based on craters
        healthyAreaLeft = 0;
        healthyAreaRight = 0;

        foreach (var crater in craters) {
            Vector3 craterPosition = crater.transform.position;
            if (craterPosition.x < 0) {
                healthyAreaLeft += crater.GetComponent<Collider2D>().bounds.size.x;
            } else {
                healthyAreaRight += crater.GetComponent<Collider2D>().bounds.size.x;
            }
        }

    }
    IEnumerator LoadNextBomb(float secondsDelay){

        // Debug.Log("==>LoadNextBoomb");
        yield return new WaitForSeconds(secondsDelay);    
        InitBomb();    
    }


    private void SetPlayerHomeCorners (PlayerMovement playerMovement) {
        float width = playerMovement.playerHome.GetComponent<BoxCollider2D>().bounds.size.x;
        float height = playerMovement.playerHome.GetComponent<BoxCollider2D>().bounds.size.y;
        // float width = go.GetComponent<BoxCollider2D> ().bounds.size.x;
        // float height = go.GetComponent<BoxCollider2D> ().bounds.size.y;

        Vector2 topRight = playerMovement.playerHome.transform.position, topLeft = playerMovement.playerHome.transform.position, bottomRight = playerMovement.playerHome.transform.position, bottomLeft = playerMovement.playerHome.transform.position;
        //Vector2 topRight = go.transform.position, topLeft = go.transform.position, bottomRight = go.transform.position, bottomLeft = go.transform.position;

        topRight.x += width / 2;
        topRight.y += height / 2;

        topLeft.x -= width / 2;
        topLeft.y += height / 2;

        bottomRight.x += width / 2;
        bottomRight.y -= height / 2;

        bottomLeft.x -= width / 2;
        bottomLeft.y -= height / 2;
        playerMovement.homeTopLeft = new Vector2(topLeft.x, topLeft.y);
        playerMovement.homeBottomRight = new Vector2(bottomRight.x, bottomRight.y);

    }

    private void OnDrawGizmos()
    {
        // Draw the home corners for each player in the editor
        foreach (var player in players)
        {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null)
            {
                if(playerMovement.isRightSide)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.cyan;

                Gizmos.DrawLine(playerMovement.homeTopLeft, new Vector2(playerMovement.homeTopLeft.x, playerMovement.homeBottomRight.y));

                Gizmos.DrawLine(playerMovement.homeTopLeft, new Vector2(playerMovement.homeBottomRight.x, playerMovement.homeTopLeft.y));

                Gizmos.DrawLine(playerMovement.homeBottomRight, new Vector2(playerMovement.homeTopLeft.x, playerMovement.homeBottomRight.y));

                Gizmos.DrawLine(playerMovement.homeBottomRight, new Vector2(playerMovement.homeBottomRight.x, playerMovement.homeTopLeft.y));

            }
        }
    }

}
