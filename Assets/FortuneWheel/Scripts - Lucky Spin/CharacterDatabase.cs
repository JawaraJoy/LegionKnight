using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase instance { get; private set; }
    public bool IsDataChange { get; private set; }

    public List<string> GetOwnedCharacterID;
    public bool IsDataChanged = false;
    private string characterCommon1;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public bool CheckOwnedCharacter(CharacterScriptableObj character)
    {
        bool Find = false;
        for (int i = 0; i < GetOwnedCharacterID.Count; i++)
        {
            if (GetOwnedCharacterID[i] == character.CharacterID)
            {
                Find = true;
                break;
            }
        }
        return Find;
    }

    void Start()
    {
    }
    private void Update()
    {
        if (IsDataChanged) Save();
        
    }
    public List<string> Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerDatabase.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/playerDatabase.dat", FileMode.Open);
            Data_CharacterDatabase data = (Data_CharacterDatabase)bf.Deserialize(file);
            GetOwnedCharacterID = new List<string>(data.ownedCharacterID);
            file.Close();
            return GetOwnedCharacterID;
        }
        return null;

    }

    public void Save()
    {
        IsDataChanged = false;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Application.persistentDataPath + "/playerDatabase.dat", FileMode.Create);
        Data_CharacterDatabase data = new Data_CharacterDatabase();
        data.ownedCharacterID = new List<string>(GetOwnedCharacterID);
        bf.Serialize(file, data);
        file.Close();
    }

    public void resetData() {
        GetOwnedCharacterID.Clear();
        GetOwnedCharacterID.Add("characterCommon1");
        Save();
    }
}

[System.Serializable]
class Data_CharacterDatabase
{
    public List<string> ownedCharacterID;
    //public string ownedCharacterID;
}

