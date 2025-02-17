using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataName : MonoBehaviour
{

    public GameObject DefaultNameText;

    public static int intDefaultNameText;
    // Start is called before the first frame update
    void Start()
    {
        intDefaultNameText = PlayerPrefs.GetInt("intDefNameText", 1);
    }

    void Update()
    {
        if (intDefaultNameText == 1){
            DefaultNameText.SetActive(true);
        }
        if (intDefaultNameText == 2){
            DefaultNameText.SetActive(false);
        }
    }
}
