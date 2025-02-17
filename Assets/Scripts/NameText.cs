using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameText : MonoBehaviour
{
    public string theName;
    public InputField inputField;

    public GameObject inputFieldText;
    public Text textDisplay;
    public GameObject defaultNameText, CreateNamePanel;
    [SerializeField] Button buttonChangeName;

    //public int theName;

    void Start() {
        theName = PlayerPrefs.GetString("Name", theName);
        textDisplay.GetComponent<Text>().text = "Hi, " + theName;
    }

    void Update() {
        if (inputField.text.Length > 0) {
			buttonChangeName.interactable = true;
		}
		else if (inputField.text.Length == 0){
            buttonChangeName.interactable = false;
        }
    }
    public void ChangeName(){
        theName = inputFieldText.GetComponent<Text>().text;
        textDisplay.GetComponent<Text>().text = "Hi, " + theName;
        PlayerPrefs.SetString("Name", theName);

        if(DataName.intDefaultNameText == 1) // checks if you have the item
        {
            DataName.intDefaultNameText = 2;
            PlayerPrefs.SetInt("intDefNameText", DataName.intDefaultNameText); // saves the gameobject
            
        }

        CreateNamePanel.SetActive(false);

        
    }

    public void OpenCreateNamePanel(){
        CreateNamePanel.SetActive(true);
    }

}
