using UnityEngine;

public class Stats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerData playerData;
    void Start()
    {
        playerData = PlayerData.GetInstance();
        if (playerData != null)
        {
            Debug.Log($"Stats => PlayerData details: {JsonUtility.ToJson(playerData)}");
        }else{
            Debug.Log("Stats => PlayerData is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
