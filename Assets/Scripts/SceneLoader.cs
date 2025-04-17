using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 3f;
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    public GameObject SceneLoaderPrefab;
    SceneLoader instance;

    void Awake()
    {
        // int numbsSceneLoader = FindObjectsByType<SceneLoader>(FindObjectsSortMode.None).Length;
        // // FindObjectsByType<GameSession>().Length;
        // if (numbsSceneLoader>1){
        //     Destroy(gameObject);
        // }else{
        //     DontDestroyOnLoad(gameObject);
        // } 
    }


    void Start()
    {
       // instance = Instantiate(SceneLoaderPrefab).GetComponent<SceneLoader>();
        
    }
    public void LoadGame(String levelName){
        // SceneManager.LoadScene(1);
        // ScoreKeeper.GetInstance().Score = 0;
      //  Debug.Log("Active? "+gameObject.activeInHierarchy);
        StartCoroutine(LoadLevel(levelName));
        // SceneManager.LoadScene("Level1");
        // SceneManager.LoadScene("SummerNight1");
    }
    public void LoadMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadGameOver(){
        StartCoroutine(WaitAndLoad("GameOver",sceneLoadDelay));
        // SceneManager.LoadScene("GameOver");
    }
    public void LoadLevelScene(){
        SceneManager.LoadScene("LevelSelect");
    }
    public void QuitGame(){
        Application.Quit();
    }
    // public void LoadLevel(int level){
    //     // ScoreKeeper.GetInstance().Score = 0;
    //     SceneManager.LoadScene("Level"+level.ToString());
    // }

    IEnumerator WaitAndLoad(string sceneName, float delay){
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadLevel(String level){
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(level.ToString());
        // SceneManager.LoadScene("Level"+level.ToString());
    }
}
