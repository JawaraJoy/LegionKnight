using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*LIST LANGUAGE
 * 
 * ENGLISH = 0
 * INDONESIA = 1
 * 
 */
public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;
    [SerializeField]private int languageIndex;
    public int GetLanguageIndex() { return languageIndex; }

    public delegate void OnLanguageChange(int _languangeIndex);
    public OnLanguageChange onLanguageChange;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (PlayerPrefs.HasKey("Language"))
        {
            languageIndex = PlayerPrefs.GetInt("Language");
        }
        else
        {
            languageIndex = 0;
            PlayerPrefs.SetInt("Language", languageIndex);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetLanguage(int type)
    {
        languageIndex = type;
        PlayerPrefs.SetInt("Language", languageIndex);
        onLanguageChange?.Invoke(type);
    }
}
