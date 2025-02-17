using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

public class SaveAndLoad : MonoBehaviour
{
    /*public void SaveToJSON<T>(List<T> toSave, string filename)
    {
        Debug.Log(GetPath(filename));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public void SaveToJSON<T>(T toSave, string filename)
    {
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(filename), content);
    }

    public List<T> ReadListFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return null;
        }
        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;
    }

    public  T ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);

        return res;

    }

    private  string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename+".json";
    }

    private  void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private  string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }*/
}

[System.Serializable]
public class IsiData
{
    public string sceneName;
    
}
public static class JsonHelper
{
    /*public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }*/
}
