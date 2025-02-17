using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {

    public GameObject LockedCharacterObj;

    public Image highlightCharacter;
    public GameObject CharacterPreview;
    public GameObject CharacterSpawn;
    public GameObject newInstance;
    public CharacterScriptableObj currentCharacter;

    public GameObject CharacterCardPrefab;
    public List<CharacterScriptableObj> AllCharacter;
    public List<CharacterScriptableObj> AllCharacterRare;
    public List<CharacterScriptableObj> AllCharacterEpic;
    public List<CharacterScriptableObj> AllCharacterLegendary;
    public List<CharacterScriptableObj> All = new List<CharacterScriptableObj>();

    public Transform SpawnAll;
    public Button AllButton, CommonButton, RareButton, EpicButton, LegendaryButton;

    void Start()
    {
        newInstance = GameObject.FindGameObjectWithTag("Player");
        #region All Character
        for (int i = 0; i < AllCharacter.Count; i++)
        {
            All.Add(AllCharacter[i]);
        }
        for (int i = 0; i < AllCharacterRare.Count; i++)
        {
            All.Add(AllCharacterRare[i]);
        }
        for (int i = 0; i < AllCharacterEpic.Count; i++)
        {
            All.Add(AllCharacterEpic[i]);
        }
        for (int i = 0; i < AllCharacterLegendary.Count; i++)
        {
            All.Add(AllCharacterLegendary[i]);
        }
        for (int i = 0; i < All.Count; i++)
        {
            GameObject newCharacter = Instantiate(CharacterCardPrefab, SpawnAll);
            SelectCharacterList newCharacterScript = newCharacter.GetComponent<SelectCharacterList>();
            newCharacterScript.myCharacter = All[i];
            newCharacterScript.CharacterIcon.sprite = newCharacterScript.myCharacter.CharacterIcon;
            newCharacterScript.cs = this;
            //newCharacterScript.Refresh(CharacterDatabase.instance.CheckOwnedCharacter(newCharacterScript.myCharacter));
        }
        #endregion
        #region All button panel
        AllButton.onClick.AddListener(() =>
        {
            Starting();
        });
        CommonButton.onClick.AddListener(() => ShowCharacter(Rarity.common));
        RareButton.onClick.AddListener(() => ShowCharacter(Rarity.rare));
        EpicButton.onClick.AddListener(() => ShowCharacter(Rarity.epic));
        LegendaryButton.onClick.AddListener(() => ShowCharacter(Rarity.legendary));
        #endregion
        Starting();
    }
    public void Starting()
    {
        ResetAll();
        for (int i = 0; i < SpawnAll.childCount; i++)
        {
            SpawnAll.GetChild(i).gameObject.SetActive(true);
            for (int j = 0; j < CharacterDatabase.instance.GetOwnedCharacterID.Count; j++)
            {
                if (SpawnAll.GetChild(i).GetComponent<SelectCharacterList>().myCharacter.CharacterID == CharacterDatabase.instance.GetOwnedCharacterID[j])
                {
                    SpawnAll.GetChild(i).GetComponent<SelectCharacterList>().Refresh(true);

                }
            }
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < SpawnAll.childCount; i++)
        {
            for (int j = 0; j < CharacterDatabase.instance.GetOwnedCharacterID.Count; j++)
            {
                if(SpawnAll.GetChild(i).GetComponent<SelectCharacterList>().myCharacter.CharacterID == CharacterDatabase.instance.GetOwnedCharacterID[j])
                {
                    SpawnAll.GetChild(i).GetComponent<SelectCharacterList>().Refresh(true);

                }
            }
        }

    }
    private void ShowCharacter(Rarity rarity)
    {
        ResetAll();
        for (int i = 0; i < SpawnAll.childCount; i++)
        {
            GameObject a = SpawnAll.GetChild(i).gameObject;
            if(a != null)
            {
                if (a.GetComponent<SelectCharacterList>().myCharacter.CharacterRarity == rarity)
                {
                    a.SetActive(true);
                }
            }

        }
        Refresh();
    }

    void Update()
    {
    }
    public void ResetAll()
    {
        for (int i = 0; i < SpawnAll.childCount; i++)
        {
            SpawnAll.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void SelectHighlight(CharacterScriptableObj CharacterChange){
        currentCharacter = CharacterChange;
        highlightCharacter.sprite = CharacterChange.CharacterIcon;
        CharacterPreview = CharacterChange.CharacterPrefab;
        //Destroy(CharacterPreview);
        DeletePreview();
        newInstance = Instantiate(CharacterPreview);
        CharacterPreview.transform.position = CharacterSpawn.transform.position;
        //Instantiate(CharacterPreview, CharacterSpawn);
    }

    public void DeletePreview()
    {
        Destroy(newInstance);
    }
    

    public void LockedCharacter(){
        LockedCharacterObj.SetActive(true);
        StartCoroutine(LockedCharacterHide());
    }

    IEnumerator LockedCharacterHide(){
        yield return new WaitForSeconds(1f);
        LockedCharacterObj.SetActive(false);
    }
}