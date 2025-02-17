using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BuyObstacle : MonoBehaviour
{
    /*public GameObject PriceObj, SelectButton, KeteranganObj;
    public int intSpawnerObs;
    public int HargaObs;
    public bool PunyaObs;

    

    public String SpawnerObsString;

    public GameObject GameManagerObj;
    // Start is called before the first frame update
    void Start()
    {
        intSpawnerObs = PlayerPrefs.GetInt(SpawnerObsString, intSpawnerObs);
    }

    // Update is called once per frame
    void Update()
    {
        
        // Beli
        if (intSpawnerObs == 1){ //Sebelum Dibeli / ketika direset
            PunyaObs = false;
            PriceObj.SetActive(true);
            KeteranganObj.SetActive(false);
            SelectButton.GetComponent<Image>().raycastTarget = false;
        }

        if (intSpawnerObs == 2){ //Sudah Dibeli
            PunyaObs = true;
            PriceObj.SetActive(false);
            KeteranganObj.SetActive(true);
            SelectButton.GetComponent<Image>().raycastTarget = true;
        }

        /*if (Input.GetKeyDown("x"))
        {
            PlayerPrefs.DeleteAll();
            Application.LoadLevel(1);
        }*/
    /*}

    public void BeliObs(){
        if(intSpawnerObs == 1) // checks if you have the item
        {
            if(GameManager.Coin < HargaObs) //change 300 to cost
            {
                GameManagerObj.GetComponent<CharSelection>().ShowPeringatan();
            }

            if(GameManager.Coin >= HargaObs) //change 300 to cost
            {
                GameManager.Coin -= HargaObs;
                GameManagerObj.GetComponent<CharSelection>().BuyEffect();
                intSpawnerObs = 2;
                PlayerPrefs.SetInt(SpawnerObsString, intSpawnerObs); // saves the gameobject
                PlayerPrefs.SetInt("Coin", GameManager.Coin); // saves the gameobject
            }
        }
    }

    */

}
