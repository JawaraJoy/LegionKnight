using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<IsiData> nama = new List<IsiData>();
    private void Start()
    {
        IsiData temp = new IsiData { sceneName = "budi" };
        nama.Add(temp);
        
    }
}
