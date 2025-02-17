using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AppsDaddyO.Rewards;
using System.Collections.Generic;

public static class SaveLoadManager
{
    public static void SaveData(RewardData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/RewardData.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("saving");
    }

    public static RewardData LoadData()
    {
        string path = Application.persistentDataPath + "/RewardData.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            RewardData data = formatter.Deserialize(stream) as RewardData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
    public static int GetSavedDataCount()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = directory.GetFiles("*.dat");
        return files.Length;
    }
    public static void SaveDataTemp(RewardDataTemp data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/RewardData.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("saving");
    }

    public static RewardDataTemp LoadDataTemp()
    {
        string path = Application.persistentDataPath + "/RewardData.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            RewardDataTemp data = formatter.Deserialize(stream) as RewardDataTemp;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
}
[System.Serializable]
public class RewardData
{
    public int _NextRewardDay;
    public bool _isAvailable;
    public bool _isClaimed;
    public int _TheDay;
    public List <MDR_RewardsItem> item;
    public MDR_RewardOnItem.State[] _state;
}

[System.Serializable]
public class RewardDataTemp
{
    public RewardSlotClass[] data = new RewardSlotClass[8];
}


