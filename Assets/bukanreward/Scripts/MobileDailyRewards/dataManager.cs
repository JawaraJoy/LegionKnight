using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    private string savePath;
    public static DataManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #region DailyReward Save&Load
    public void SaveData(string[] nama, int day, int month,int year, bool claim,int today,bool login)
    {
        savePath = Application.persistentDataPath + "/save_data.json";
        // Create a new container class to hold the string and integer values
        DataContainer data = new DataContainer { nama = nama, day = day, month = month, year = year, claim = claim, today = today,login = login};

        // Convert the container class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Write the JSON string to a file
        File.WriteAllText(savePath, json);
    }
    public DataContainer loaddata()
    {
        savePath = Application.persistentDataPath + "/save_data.json";
        if (File.Exists(savePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(savePath);

            // Convert the JSON string back to the container class
            DataContainer data = JsonUtility.FromJson<DataContainer>(json);
            //load disini
            return data;
        }
        return null;
    }
    public void deletdata()
    {
        savePath = Application.persistentDataPath + "/save_data.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("File deleted.");
        }
        else
        {
            Debug.Log("File not found.");
        }
    }
    #endregion
    #region Character Save&Load
    public void SaveData(string characterName, string rarity)
    {
        savePath = Application.persistentDataPath + "/Character_Data.json";
        // Create a new container class to hold the string and integer values
        DataContainer data = new DataContainer { CharacterName = characterName, rarity = rarity };

        // Convert the container class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Write the JSON string to a file
        File.WriteAllText(savePath, json);
    }

    public DataContainer LoadCharacterData()
    {
        savePath = Application.persistentDataPath + "/Character_Data.json";
        if (File.Exists(savePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(savePath);

            // Convert the JSON string back to the container class
            DataContainer data = JsonUtility.FromJson<DataContainer>(json);
            //load disini
            return data;
        }
        return null;
    }
    #endregion
    #region PriorityLevel Save&Load
    public void SaveData(int activityTime)
    {
        savePath = Application.persistentDataPath + "/ActivityTime.json";
        // Create a new container class to hold the string and integer values
        DataContainer data = new DataContainer { activityPoint = activityTime};

        // Convert the container class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Write the JSON string to a file
        File.WriteAllText(savePath, json);
    }
    public DataContainer LoadActivityTime()
    {
        savePath = Application.persistentDataPath + "/ActivityTime.json";
        if (File.Exists(savePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(savePath);
            // Convert the JSON string back to the container class
            DataContainer data = JsonUtility.FromJson<DataContainer>(json);
            //load disini
            return data;
        }
        return null;

    }
    #endregion
    #region Harga di shop Save&Load
    public void SaveData(int[] harga)
    {
        savePath = Application.persistentDataPath + "/HargaShop.json";
        // Create a new container class to hold the string and integer values
        DataContainer data = new DataContainer { hargaDiShop = harga};

        // Convert the container class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Write the JSON string to a file
        File.WriteAllText(savePath, json);
    }
    public DataContainer LoadHargaShop()
    {
        savePath = Application.persistentDataPath + "/HargaShop.json";
        if (File.Exists(savePath))
        {
            // Read the JSON string from the file
            string json = File.ReadAllText(savePath);
            // Convert the JSON string back to the container class
            DataContainer data = JsonUtility.FromJson<DataContainer>(json);
            //load disini
            return data;
        }
        return null;

    }
    #endregion
    #region Delete all save data
    public void DeleteAllData()
    {

        string path = Application.persistentDataPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        // Menghapus semua file yang ada di directory persistentDataPath
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
    }

    #endregion
}

[System.Serializable]
public class DataContainer
{
    //daily reward
    public string[] nama;
    public int day;
    public int month;
    public int year;
    public bool claim;
    public int today;
    public bool login;

    //activityPoint
    public int activityPoint; //level 0/1

    //character data
    public string CharacterName;
    public string rarity;

    //harga di shop
    public int[] hargaDiShop;
}
