using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSkipRevive : MonoBehaviour
{
    public GameObject PlayerScript;
    // Start is called before the first frame update
    void Start()
    {
        /*GameManager.Score = 0;
        PlayerPrefs.SetInt("Score", GameManager.Score);*/
        
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerScript = GameObject.FindGameObjectWithTag("Player");
        Destroy(PlayerScript);
        SceneManager.LoadScene("Game");
    }
}
