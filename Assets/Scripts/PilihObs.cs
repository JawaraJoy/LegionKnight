using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PilihObs : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnObsList;

    
    public int i;

    public int intSave;

    void Start(){
        intSave = PlayerPrefs.GetInt("SaveObs"); // load the gameobject*/

    }

    void Update() {
        
        
        if (intSave == 0){ //Sebelum Dipilih
            
        }

        if (intSave == 1){ //Sudah Dipilih
            Debug.Log("SpawnObs 1 Saved");
            
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(1);
        }

        /*if (Input.GetKeyDown("x"))
        {
            PlayerPrefs.DeleteAll();
            Application.LoadLevel(1);
        }*/

        

    }

    void ResetGameObject(){
        for(i = 0; i < SpawnObsList.Length; i++){
            if(SpawnObsList[i].activeSelf) SpawnObsList[i].SetActive(false);
            
        }
    }

    public void OpenSpawnObs(int index){
        ResetGameObject();
        SpawnObsList[index].SetActive(true);
        PlayerPrefs.SetInt("SaveObs", index); // saves the gameobject
        intSave = index;
    }

}