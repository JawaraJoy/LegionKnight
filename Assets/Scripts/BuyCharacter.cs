using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// /////////////////////////Script Gajadi
/// </summary>
public class BuyCharacter : MonoBehaviour
{
    /*public GameObject Peringatan;
    public GameObject LockedChar;

    public GameObject Char;
    public int HargaChar;
    
    public GameObject BuyButton2, BuyButton3, BuyButtonSC1, BuyButtonSC2, BuyButtonSC3, BuyButtonSC4, BuyButtonSC5, BuyButtonSC6;

    public bool CharOwn;

    public static int intCharOwn;

    // Start is called before the first frame update
    void Start()
    {
    
        CharOwn = true;

        intCharOwn = PlayerPrefs.GetInt("CharOwn", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("-")){
            
            if (GameManager.Coin <= HargaChar){
                Debug.Log("Teu Boga Duit");
            }
            if (GameManager.Coin >= HargaChar){
                GameManager.Coin -= HargaChar;
            }
        }
        if (Input.GetKeyDown("=")){
            GameManager.Coin += HargaChar;
        }

        //Buy Character2//////////////////////////////////////////////////////////
        if (intCharOwn == 1){

            CharOwn = false;
            
            
        }
        if (intCharOwn == 2){

            CharOwn = true;
            BuyButton2.SetActive(false);
            

            //LockedChar.SetActive(false);

            //Destroy Claim Char
            //Destroy(ClaimChar2Panel);
        }
    }

    public void DestroyPlayer(){
        //Destroy(PlayerPrefab);
    }

    public void SelectChar(){
        //Destroy(PlayerPrefab);
        //Instantiate(Char1, spawnPos.transform.position, spawnPos.transform.rotation);
        Char.SetActive(true);
        if (intCharOwn == 1){

            BuyButton2.SetActive(true);
            
        }
        

        if (CharOwn == false){
            LockedChar.SetActive(true);
            FalseBuyButtonAll();
        }

        if (CharOwn == true){
            LockedChar.SetActive(false);
        }

        //FalseBuyButtonAll();
    }


    public void FalseBuyButtonAll(){
        BuyButton2.SetActive(false);
        BuyButton3.SetActive(false);
        BuyButtonSC1.SetActive(false);
        BuyButtonSC2.SetActive(false);
        BuyButtonSC3.SetActive(false);
        BuyButtonSC4.SetActive(false);
        BuyButtonSC5.SetActive(false);
        BuyButtonSC6.SetActive(false);
    }

    public void SelectChar1(){
        FalseBuyButtonAll();
    }

    public void SelectChar2(){
        BuyButton2.SetActive(true);
        BuyButton3.SetActive(false);
        BuyButtonSC1.SetActive(false);
        BuyButtonSC2.SetActive(false);
        BuyButtonSC3.SetActive(false);
        BuyButtonSC4.SetActive(false);
    }





    //Buy Char /////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void BuyChar(){
        if (GameManager.Coin <= HargaChar){
                Peringatan.SetActive(true);
                StartCoroutine(HidePeringatan());
        }

        if (GameManager.Coin >= HargaChar){
            GameManager.Coin -= HargaChar;
            LockedChar.SetActive(false);
            if(BuyCharacter.intCharOwn == 1) // checks if you have the item
            {
                BuyCharacter.intCharOwn = 2;
                PlayerPrefs.SetInt("CharOwn", BuyCharacter.intCharOwn); // saves the gameobject
            }
        }
    }

    IEnumerator HidePeringatan(){
        yield return new WaitForSeconds(1f);
        Peringatan.SetActive(false);
    }*/
}
