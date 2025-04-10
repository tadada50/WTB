using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    [SerializeField] float sceneLoadDelay = 3f;
    public void LoadGame(){
        // SceneManager.LoadScene(1);
        // ScoreKeeper.GetInstance().Score = 0;
        SceneManager.LoadScene("SummerNight1");
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
    public void LoadLevel(int level){
        // ScoreKeeper.GetInstance().Score = 0;
        SceneManager.LoadScene("Level"+level.ToString());
    }

    IEnumerator WaitAndLoad(string sceneName, float delay){
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
