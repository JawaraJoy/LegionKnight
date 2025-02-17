using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacter : MonoBehaviour
{
    [SerializeField] CharacterScriptableObj PlayerSelected;
    public void SetPlayer(CharacterScriptableObj obj) { PlayerSelected = obj; }
    public CharacterScriptableObj GetPlayer() { return PlayerSelected; }



    [SerializeField] Transform spawnPreview;



    public static SelectedCharacter Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
       StartCoroutine(Load());
    }
    public void Save()
    {
        DataManager.instance.SaveData(PlayerSelected.name,PlayerSelected.CharacterRarity.ToString());
    }
    IEnumerator Load()
    {
        DataContainer datalist = DataManager.instance.LoadCharacterData();
        if(datalist == null)
        {
            Save();
            StartCoroutine(Load());
        }

        yield return new WaitForSeconds(0.1f);

        if(datalist.rarity == "common")
            PlayerSelected = Resources.Load<CharacterScriptableObj>("CharacterList/Common/"+ datalist.CharacterName);
        else if (datalist.rarity == "epic")
            PlayerSelected = Resources.Load<CharacterScriptableObj>($"CharacterList/Epic/{datalist.CharacterName}");
        else if (datalist.rarity == "legendary")
            PlayerSelected = Resources.Load<CharacterScriptableObj>($"CharacterList/Legendary/{datalist.CharacterName}");
        else if (datalist.rarity == "rare")
            PlayerSelected = Resources.Load<CharacterScriptableObj>($"CharacterList/Rare/{datalist.CharacterName}");
        Instantiate(PlayerSelected.CharacterPrefab, spawnPreview.position, Quaternion.identity);
        ProfilePicture.Instance.SetPP(PlayerSelected.CharacterIcon);
    }
}
