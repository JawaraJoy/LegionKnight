using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public int IniLevelBerapa;

    public GameObject KunciLevel;
    public GameObject confirmPanel,errorTeks,infoKeyPanel;
    
    public Button unlockLevelBTN,noBTN,yesBTN;
    
    public int hargaLevelIni;
    
    public bool isUnlocked;
    public bool isReleased;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Level" + IniLevelBerapa))
            isUnlocked = PlayerPrefs.GetInt("Level" + IniLevelBerapa) == 1 ? true : false;
        else
            PlayerPrefs.SetInt("Level" + IniLevelBerapa, 0);
        if (!isUnlocked)
        {
            yesBTN.onClick.AddListener(() =>
            {
                isUnlocked = true;
                PlayerPrefs.SetInt("Level" + IniLevelBerapa, 1);
                FindObjectOfType<GameManager>().KeyDummyTambah(-hargaLevelIni);
                KunciLevel.SetActive(false);
                confirmPanel.SetActive(false);
            });
            noBTN.onClick.AddListener(() =>
            {
                confirmPanel.SetActive(false);
            });
            unlockLevelBTN.onClick.AddListener(() =>
            {
                if(isReleased)
                {
                    if (FindObjectOfType<GameManager>().Key >= hargaLevelIni)
                    {
                        confirmPanel.SetActive(true);
                    }
                    else
                    {
                        infoKeyPanel.SetActive(true);
                    }

                }
                else
                {
                    StartCoroutine(Errorwarn("This level not ready yet"));
                }
            });
            
        }
        else
        {
            KunciLevel.SetActive(false);
            confirmPanel.SetActive(false);

        }
    }

    private IEnumerator Errorwarn(string msg)
    {
        errorTeks.SetActive(true);
        errorTeks.GetComponentInChildren<Text>().text = msg;
        yield return new WaitForSeconds(1);
        errorTeks.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
