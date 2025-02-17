using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterList : MonoBehaviour
{
    public Image CharacterIcon;
    public GameObject LockedButtonObj;
    public CharacterSelect cs;
    public CharacterScriptableObj myCharacter;


    public bool isUnlocked;
    
    public void OnButtonSelectCharacter(){
        
        if(isUnlocked){
            cs.SelectHighlight(myCharacter);
            SelectedCharacter.Instance.SetPlayer(myCharacter);
            SelectedCharacter.Instance.Save();
            ProfilePicture.Instance.SetPP(myCharacter.CharacterIcon);
        }
        else{
            cs.LockedCharacter();
        }
    }

    public void Refresh(bool Locked){
        CharacterIcon.sprite = myCharacter.CharacterIcon;
        LockedButtonObj.SetActive(!Locked);
        isUnlocked = Locked;
    }


}
