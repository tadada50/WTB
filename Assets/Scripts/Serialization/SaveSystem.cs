using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem 
{
    static string fileName = "/player.save";
    public static void SavePlayer(PlayerData player){
        Debug.Log("==>Creating save file");
        BinaryFormatter formatter = new BinaryFormatter();
        // string path = Application.persistentDataPath + "/player.save";
        string path = Application.persistentDataPath + fileName;
        FileStream stream = new FileStream(path,FileMode.Create);
        PlayerData data = new PlayerData();
        // PlayerData data = new PlayerData(scoreKeeper);
        // formatter.Serialize(stream, data);
        formatter.Serialize(stream, player);
        stream.Close();
        Debug.Log("Save file created at: " + path);
    }
    public static PlayerData LoadPlayer(){
        // string path = Application.persistentDataPath + "/player.save";
        string path = Application.persistentDataPath + fileName;
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }else{
            Debug.Log("Save file not found in"+path);
            return null;
        }
    }
    // public static void CreateSaveFile(){
    //     string path = Application.persistentDataPath + fileName;
    //     FileStream stream = new FileStream(path,FileMode.Create);
    // }
}
