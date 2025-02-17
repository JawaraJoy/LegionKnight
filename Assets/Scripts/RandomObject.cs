using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : MonoBehaviour
{
    public GameObject[] BonusCoinAds;

    // untuk di hide saja
    //public GameObject BonusCoinAds1, BonusCoinAds2, BonusCoinAds3;
    // Start is called before the first frame update

    public void RandomCoinBonusAds()
    {
        int i = Random.Range(0, BonusCoinAds.Length);
        BonusCoinAds[i].SetActive(true);
    }

    // Update is called once per frame
    void Start()
    {
        int i = Random.Range(0, BonusCoinAds.Length);
        BonusCoinAds[i].SetActive(true);
    }

    /*public void HideAllBonusCoinAds(){
        BonusCoinAds1.SetActive(false);
        BonusCoinAds2.SetActive(false);
        BonusCoinAds3.SetActive(false);
    }*/
}
