using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CinematicEndGame : MonoBehaviour
{
    public bool isEndGame;
    public static CinematicEndGame Instance;
    [HideInInspector]public int currentScene;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        isEndGame = false;
    }
    public void EndGame(int currentLevel)
    {
        isEndGame = true;
        currentScene = currentLevel;
        SceneManager.LoadScene("Game");
    }
}
