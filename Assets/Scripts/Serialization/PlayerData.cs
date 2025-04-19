using System;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    static PlayerData instance;
    // public int highestScore;
    public int numbOfBombsExploded;
    public int leftPlayerVictories;
    public int rightPlayerVictories;
    public int leftSideLandDestroyed;
    public int rightSideLandDestroyed;

    public int playerMoney;
    public DateTime noAdsUntil;
    public bool adsEnabled;
    // public int levelReached;

    // public bool achievement1;
    // public bool achievement2;

    // public PlayerData(ScoreKeeper scoreKeeper){
    //     // highestScore = scoreKeeper.GetHighestScore();
    //     playerMoney = 0;
    //     noAdsUntil = DateTime.Now;
    //     adsEnabled = true;
    // }

    public static PlayerData GetInstance(){
        if(instance == null){
            instance = SaveSystem.LoadPlayer();
            if(instance == null){
                instance = new PlayerData();
                instance.numbOfBombsExploded = 0;
                instance.leftPlayerVictories = 0;
                instance.rightPlayerVictories = 0;
                instance.leftSideLandDestroyed = 0;
                instance.rightSideLandDestroyed = 0;
                instance.playerMoney = 0;
                instance.noAdsUntil = DateTime.Now;
                instance.adsEnabled = true;
                SaveSystem.SavePlayer(instance);
            }
        }
        return instance;
    }
    public static void SavePlayerData(){
        SaveSystem.SavePlayer(instance);
    }
    public void IncrementBombsExploded(){
        numbOfBombsExploded++;
    }
    public void IncrementLeftPlayerVictories(){
        leftPlayerVictories++;
    }   
    public void IncrementRightPlayerVictories(){
        rightPlayerVictories++;
    }
    public void IncrementLeftSideLandDestroyed(int areaDestroyed){
        leftSideLandDestroyed+=areaDestroyed;
    }
    public void IncrementRightSideLandDestroyed(int areaDestroyed){
        rightSideLandDestroyed+=areaDestroyed;
    }
    // void Awake()
    // {
    //     ManageSingleTon();
    //     GetSaveStats();
    // }
    // void ManageSingleTon()
    // {
    //     if(instance!= null){
    //         gameObject.SetActive(false);
    //         Destroy(gameObject);
    //     }else{
    //         instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     // if (FindObjectsByType<ScoreKeeper>(FindObjectsSortMode.None).Length > 1)
    //     // {
    //     //     Destroy(gameObject);
    //     //     return false;
    //     // }
    //     // DontDestroyOnLoad(gameObject);
    //     // return true;
    // }
    // public void GetSaveStats(){
    //     // PlayerData data = SaveSystem.LoadPlayer();
    //     // if(data != null){
    //     //     highestScore = data.highestScore;
    //     // }else{
    //     //     highestScore = 0;
    //     // }
    // }
    
}
