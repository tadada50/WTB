using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

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
    [SerializeField] RandomGadgetSpawner randomGadgetSpawner;
    [SerializeField] GameState currentGameState = GameState.MainMenu;
    [SerializeField] List<GameObject> players;
    // [SerializeField] int bombNumber = 1; // Number of bombs to drop
    // [SerializeField] float bombDropInterval = 2.0f; // Interval between bomb drops
    // [SerializeField] GameObject bombPrefab; // Prefab for the bomb
    [SerializeField] GameObject craterPrefab; // Prefab for the crater
    [SerializeField] GameObject gameOverPanel;
    // [SerializeField] GameObject rightPlayerHome; // Prefab for the right player home
    [SerializeField] List<GameObject> playerHomes; // Prefab for the left player home
    [SerializeField] List<GameObject> bombPrefabs; // Prefab for the left player home
    [SerializeField] GameObject scoreKeeper;
    [SerializeField] int timerMin = 10;
    [SerializeField] int timerMax = 31;
    bool leftPlayerWon = false;
    ScoreKeeper scoreKeeperScript;
    // float healthyAreaLeft;
    // float healthyAreaRight;

    // float areaSlice = 1f;
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
            scoreKeeperScript = scoreKeeper.GetComponent<ScoreKeeper>();
            scoreKeeperScript.OnRightPlayerHomeSliderValueChange += RightPlayerHomeSliderValueChangedHandler;
            scoreKeeperScript.OnLeftPlayerHomeSliderValueChange += LeftPlayerHomeSliderValueChangedHandler;
            scoreKeeperScript.OnLeftPlayerLifesChange += LeftPlayerLifesChangeHandler;
            scoreKeeperScript.OnRightPlayerLifesChange += RightPlayerLifesChangeHandler;
            StartCoroutine(WaitForTransition(1.2f));
            
    }
    IEnumerator WaitForTransition(float delay){
        yield return new WaitForSeconds(delay);
        currentGameState = GameState.BombDropping;
    }

    // public void SceneTransitionOver(){
    //     currentGameState = GameState.BombDropping;
    // }
    // Update is called once per frame
    void Update()
    {
        if(currentGameState == GameState.GameOver)
        {
            // Handle game over logic here
            // For example, you might want to reset the game or show a game over screen
           // Debug.Log("Game Over!"); // Placeholder for game over logic
            foreach (var player in players)
            {
                // player.GetComponent<PlayerMovement>().SetActive(true);
                player.GetComponentInChildren<PlayerMovement>().SetActive(false);
                if(player.GetComponentInChildren<PlayerMovement>().isRightSide){
                    if(!leftPlayerWon){ // rightsidewon
                        // player.GetComponentInChildren<PlayerMovement>().SetActive(true);
                        player.GetComponentInChildren<PlayerMovement>().GameOver();
                    }else{
                        // player.GetComponentInChildren<PlayerMovement>().SetActive(false);
                    }
                }else{
                    if(leftPlayerWon){ // lefttsidewon
                        player.GetComponentInChildren<PlayerMovement>().GameOver();
                    }else{
                        // player.GetComponentInChildren<PlayerMovement>().SetActive(false);
                    }
                }
            }
            if (!gameOverPanel.activeSelf){
                gameOverPanel.SetActive(true);
            }
               // gameOverPanel.SetActive(true);
            // currentGameState = GameState.BombDropping;
        }else if(currentGameState == GameState.PlayerActive)
        {
            // Handle player active logic here
            foreach (var player in players)
            {
                // player.GetComponent<PlayerMovement>().SetActive(true);
                player.GetComponentInChildren<PlayerMovement>().SetActive(true);
            }
        }else if(currentGameState == GameState.BombDropping)
        {   
            //instantiate bomb
            StartCoroutine(LoadNextBomb(1f));
            ReactivateGadgets();

            //drop bomb
            // Countdown
            currentGameState = GameState.PlayerActive; // Change game state to PlayerActive after dropping bombs
        }
    }
    void ReactivateGadgets(){
        GameObject[] gadgets = GameObject.FindGameObjectsWithTag("Gadget");
        foreach (GameObject gadget in gadgets)
        {
            gadget.GetComponent<GadgetBehavior>().ActivateGadget();
        }
        //remove random gadgets
        randomGadgetSpawner.RemoveAllRandomGadgets();
    }
    void InitBomb(){
            // Debug.Log("==>InitBomb");
            int randomIndex = Random.Range(0, bombPrefabs.Count);
            // bombPrefab = bombPrefabs[randomIndex];
            GameObject bombInstance = Instantiate(bombPrefabs[randomIndex], new Vector3(0, 0, 0), Quaternion.identity);
            Bomb bomb = bombInstance.GetComponent<Bomb>();
            bomb.SetTimer((float)Random.Range(timerMin, timerMax));
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
        Debug.Log($"Bomb exploded in quadrant {quadrant}  at position {position}");
        currentGameState = GameState.GameOver;
    }
    void BombExplodeHandler(Vector2 position, Bomb bomb){

        // bomb explodes where bomb body is, but the crater is where the shadow is
        bombs.Remove(bomb);

        float bombspriteHeight=0f;
        Transform[] obs = bomb.GetComponentsInChildren<Transform>();
        foreach (var ob in obs) {
            if(ob.CompareTag("BombBody")){
                bombspriteHeight = ob.GetComponent<SpriteRenderer>().bounds.size.y;
                Debug.Log($"Bomb height: {bombspriteHeight}");
                break;
            }
        }


        // Calculate distance between bomb and crater position
        float distance = Vector2.Distance(bomb.transform.position, position);
        
        // Scale crater size inversely with distance
        float scale = Mathf.Max(0.2f, 2.0f - (distance * 0.2f)); // Adjust multiplier (0.1f) to control scaling rate
        // Vector2 craterPosition = bomb.transform.position + new Vector3(0, -bombspriteHeight/2, 0); // Adjust the height of the crater position
        
        Vector2 craterPosition = bomb.transform.position + new Vector3(0,0, 0); // Adjust the height of the crater position
        GameObject craterInstance = Instantiate(craterPrefab, craterPosition, Quaternion.identity);
        craterInstance.transform.localScale = new Vector3(scale, scale, 1);

        foreach (var playerHome in playerHomes) {
            PlayerHome home = playerHome.GetComponentInChildren<PlayerHome>();
            home.RemovedBombedArea(craterInstance);
        }

        CheckIfBombHitPlayer(position, bomb.bombExplosionRadius);
        // currentGameState = GameState.GameOver;
        if(currentGameState == GameState.GameOver)
        {
            // // Handle game over logic here
            // // For example, you might want to reset the game or show a game over screen
            // // Debug.Log("Game Over!"); // Placeholder for game over logic
            // foreach (var player in players)
            // {
            //     // player.GetComponent<PlayerMovement>().SetActive(true);
            //     player.GetComponentInChildren<PlayerMovement>().SetActive(false);
            // }
            // gameOverPanel.SetActive(true);
            // Debug.Log("Game Over Panel Activated");
        }
        else
        {
            // Handle bomb explosion logic here
            currentGameState = GameState.BombDropping;
        }
        
    }

    IEnumerator LoadNextBomb(float secondsDelay){

        // Debug.Log("==>LoadNextBoomb");
        yield return new WaitForSeconds(secondsDelay);    
        InitBomb();    
    }
    private void CheckIfBombHitPlayer(Vector2 position, float explosionRadius) {
        foreach (var player in players) {
            var playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null) {
                float distanceToPlayer = Vector2.Distance(position, (Vector2)playerMovement.transform.position);
                if (distanceToPlayer < explosionRadius) { // Adjust this radius as needed
                    // currentGameState = GameState.GameOver;
                    if(playerMovement.isRightSide){
                        scoreKeeper.GetComponent<ScoreKeeper>().RightPlayerLifesCount -= 1;
                    }else{  
                        scoreKeeper.GetComponent<ScoreKeeper>().LeftPlayerLifesCount -= 1;
                    }
                }
            }
        }
    }
    private void LeftPlayerHomeSliderValueChangedHandler(float value)
    {
        Debug.Log($"LeftPlayerHomeSlider: {value}, destructionThreshold: {scoreKeeperScript.destructionThreshold}");
        if(value <= scoreKeeperScript.destructionThreshold)
        {
            // Handle left player home slider value change
            // currentGameState = GameState.GameOver;
            Debug.Log("==>LeftPlayerHomeSliderValueChangedHandler: " + value);
            // Handle left player home slider value change
            currentGameState = GameState.GameOver;
            leftPlayerWon = false;
        }
    }

    private void RightPlayerHomeSliderValueChangedHandler(float value)
    {
        Debug.Log($"RightPlayerHomeSlider: {value}, destructionThreshold: {scoreKeeperScript.destructionThreshold}");
        if(value <= scoreKeeperScript.destructionThreshold)
        {
            // Handle right player home slider value change
            // currentGameState = GameState.GameOver;
            Debug.Log("==>RightPlayerHomeSliderValueChangedHandler: " + value);
            // Handle right player home slider value change
            currentGameState = GameState.GameOver;
            leftPlayerWon = true;
        }
    }

    private void LeftPlayerLifesChangeHandler(int value)
    {
        if(value <= 0)
        {
            // Handle left player lives change
            Debug.Log("==>LeftPlayerLifesChangeHandler: " + value);
            currentGameState = GameState.GameOver;
            leftPlayerWon = false;
        }
    }

    private void RightPlayerLifesChangeHandler(int value)
    {
        if(value <= 0)
        {

            // Handle left player lives change
            Debug.Log("==>RightPlayerLifesChangeHandler: " + value);
            currentGameState = GameState.GameOver;
            leftPlayerWon = true;
        }
    }
    private void SetPlayerHomeCorners (PlayerHome playerHome) {
        float width = playerHome.GetComponent<BoxCollider2D>().bounds.size.x;
        float height = playerHome.GetComponent<BoxCollider2D>().bounds.size.y;
        // float width = go.GetComponent<BoxCollider2D> ().bounds.size.x;
        // float height = go.GetComponent<BoxCollider2D> ().bounds.size.y;

        Vector2 topRight = playerHome.transform.position, topLeft = playerHome.transform.position, bottomRight = playerHome.transform.position, bottomLeft = playerHome.transform.position;
        //Vector2 topRight = go.transform.position, topLeft = go.transform.position, bottomRight = go.transform.position, bottomLeft = go.transform.position;

        topRight.x += width / 2;
        topRight.y += height / 2;

        topLeft.x -= width / 2;
        topLeft.y += height / 2;

        bottomRight.x += width / 2;
        bottomRight.y -= height / 2;

        bottomLeft.x -= width / 2;
        bottomLeft.y -= height / 2;
        playerHome.homeTopLeft = new Vector2(topLeft.x, topLeft.y);
        playerHome.homeBottomRight = new Vector2(bottomRight.x, bottomRight.y);

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

    public void AddTimeToBomb(float time)
    {
        // Debug.Log($"==>Adding {time} secs. Bomb count: {bombs.Count }");
        if (bombs.Count > 0)
        {
            int randomIndex = Random.Range(0, bombs.Count);
            bombs[randomIndex].AddTime(time);
            // Debug.Log($"Added {time} seconds to bomb {randomIndex}");
        }
    }
    public void RevealBombTime(){
        for(int i = 0; i < bombs.Count; i++)
        {
            bombs[i].RevealTime();
        }
    }

    public void AddRandomGadget(GadgetBehavior gadgetBehavior, bool isRightSide){
        // Debug.Log($"==>Adding random gadget: {gadgetBehavior}");
        randomGadgetSpawner.SpawnGadget(gadgetBehavior.gameObject, isRightSide);
    }
    // private void OnDrawGizmos()
    // {
    //     // Draw the home corners for each player in the editor
    //     foreach (var player in players)
    //     {
    //         PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
    //         if (playerMovement != null)
    //         {
    //             if(playerMovement.isRightSide)
    //                 Gizmos.color = Color.red;
    //             else
    //                 Gizmos.color = Color.cyan;

    //             Gizmos.DrawLine(playerMovement.homeTopLeft, new Vector2(playerMovement.homeTopLeft.x, playerMovement.homeBottomRight.y));

    //             Gizmos.DrawLine(playerMovement.homeTopLeft, new Vector2(playerMovement.homeBottomRight.x, playerMovement.homeTopLeft.y));

    //             Gizmos.DrawLine(playerMovement.homeBottomRight, new Vector2(playerMovement.homeTopLeft.x, playerMovement.homeBottomRight.y));

    //             Gizmos.DrawLine(playerMovement.homeBottomRight, new Vector2(playerMovement.homeBottomRight.x, playerMovement.homeTopLeft.y));

    //         }
    //     }
    // }

}
