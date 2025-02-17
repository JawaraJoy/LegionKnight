using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendaryShop : MonoBehaviour
{
    public GameObject coinIcon, priceText,description;
    private void Start()
    {
        if(PlayerPrefs.HasKey("OpenLegendaryShop"))
        {
            GetComponent<Image>().color = new Color(255,255,255,255);
            GetComponent<Button>().interactable = true;
            coinIcon.SetActive(true);
            priceText.SetActive(true);
            description.SetActive(false);
        }
        else
        {
            GetComponent<Button>().interactable = false;

        }
    }
}
