using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

//
public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private NarasiTeks[] narasiStart,narasiEnd;

    [SerializeField] private int index;
    
    
    [SerializeField] Button TextBox_BTN,Skip_BTN,exitButton;
    [SerializeField] Text dialogueText;
    
    
    [SerializeField] SaveAndLoad saveAndLoad;

    public GameObject finishPanel,bg;

    public Animator anim;

    private string sceneName;
    public NARASISTATE state;
    public List<IsiData> datalist = new List<IsiData>();
    public string fileName = "Narasi";

    [SerializeField] Image icon;
    public int ModeGamePlay;

    #region singleton
    public static DialogueSystem Instance { get; private set; }
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }
    #endregion

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        ModeGamePlay = PlayerPrefs.GetInt("ModeGamePlay");
        if (ModeGamePlay == 0)
        {
            FindObjectOfType<GameManager>().MunculSpawner();
        }
        else
        {
            LoadData();
        }
        TextBox_BTN.onClick.AddListener(NextDialogue);
        Skip_BTN.onClick.AddListener(SkipDialogue);
    }

    private void SkipDialogue()
    {
        switch (state)
        {
            case NARASISTATE.START:
                TextBox_BTN.gameObject.SetActive(false);
                FindObjectOfType<GameManager>().MunculSpawner();
                index = 0;
                SaveData();
                break;
            case NARASISTATE.END:
                index = 0;
                break;
            default:
                break;
        }
    }

    private void NextDialogue()
    {
        index++;
        switch (state)
        {
            case NARASISTATE.START:
                if (index < narasiStart.Length)
                {
                    if (LanguageManager.instance.GetLanguageIndex() == 0)
                    {
                        dialogueText.text = narasiStart[index].narasiTeks;
                    }
                    else if (LanguageManager.instance.GetLanguageIndex() == 1)
                    {
                        dialogueText.text = narasiStart[index].terjemahanTeks;
                    }
                    icon.sprite = narasiStart[index].icon.sprite;
                }
                else
                {
                    TextBox_BTN.gameObject.SetActive(false);
                    FindObjectOfType<GameManager>().MunculSpawner();
                    SaveData();
                    index = 0;
                }

                break;
            case NARASISTATE.END:
                if (index < narasiEnd.Length)
                {
                    if (LanguageManager.instance.GetLanguageIndex() == 0)
                    {
                        dialogueText.text = narasiEnd[index].narasiTeks;
                    }
                    else if (LanguageManager.instance.GetLanguageIndex() == 1)
                    {
                        dialogueText.text = narasiEnd[index].terjemahanTeks;
                    }
                    icon.sprite = narasiEnd[index].icon.sprite;
                }
                else
                {
                    anim.enabled = false;
                    TextBox_BTN.gameObject.SetActive(false);
                    finishPanel.SetActive(true);
                    index = 0;
                }
                break;
            default:
                break;
        }
    }

    private void LoadData()
    {
        //datalist = saveAndLoad.ReadListFromJSON<IsiData>(fileName);
        if (datalist == null)
        {
            datalist = new List<IsiData>();
            SetShowState(NARASISTATE.START);
        }
        else
        {
            string a = CheckSceneName();
            if (a == null)
            {
                datalist = new List<IsiData>();
                SaveData();
                TextBox_BTN.gameObject.SetActive(true);
                SetShowState(NARASISTATE.START);
            }
            else
            {
                FindObjectOfType<GameManager>().MunculSpawner();
            }
        }
    }

    private void ShowText()
    {
        TextBox_BTN.gameObject.SetActive(true);
        switch (state)
        {
            case NARASISTATE.START:
                if(LanguageManager.instance.GetLanguageIndex()==0)
                {
                    dialogueText.text = narasiStart[index].narasiTeks;
                }
                else if (LanguageManager.instance.GetLanguageIndex()==1)
                {
                    dialogueText.text = narasiStart[index].terjemahanTeks;
                }
                icon.sprite = narasiStart[index].icon.sprite;
                break;
            case NARASISTATE.END:
                Skip_BTN.gameObject.SetActive(false);
                bg.SetActive(true);
                anim.enabled = true;
                if (LanguageManager.instance.GetLanguageIndex() == 0)
                {
                    dialogueText.text = narasiEnd[index].narasiTeks;
                }
                else if (LanguageManager.instance.GetLanguageIndex() == 1)
                {
                    dialogueText.text = narasiEnd[index].terjemahanTeks;
                }
                icon.sprite = narasiEnd[index].icon.sprite;
                break;
            default:
                break;
        }
    }

    public void SaveData()
    {
        IsiData temp = new IsiData { sceneName = sceneName };
        datalist.Add(temp);
        //saveAndLoad.SaveToJSON(datalist, fileName);
    }

    private string CheckSceneName()
    {
        for (int i = 0; i < datalist.Count; i++)
        {
            if(datalist[i].sceneName == sceneName)
                return datalist[i].sceneName;
        }
        return null;
    }

    public void SetShowState(NARASISTATE newstate)
    {
        state = newstate;
        ShowText();
    }
}

public enum NARASISTATE
{
    START,
    END
}

[System.Serializable]
public class NarasiTeks
{
    [TextArea(1,10)]
    public string narasiTeks;
    [TextArea(1, 10)]
    public string terjemahanTeks;

    public SpriteRenderer icon;
}