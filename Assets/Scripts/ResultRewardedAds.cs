using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRewardedAds : MonoBehaviour
{
    public static ResultRewardedAds instance;
    public Text amountText;
    public Button confirmBTN;
    private void Awake()
    {
        if (instance == null)
            instance = this;

    }
    private void Start()
    {
        confirmBTN.onClick.AddListener(() => {
            FindObjectOfType<GameManager>().DestroyPlayer();
            FindObjectOfType<GameManager>().KeGame();
            gameObject.SetActive(false);
        });
    }
    public void Result(int amount,string namahadiah)
    {
        amountText.text = amount.ToString()+$" {namahadiah}";
    }
}
