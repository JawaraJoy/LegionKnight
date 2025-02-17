using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadShop : MonoBehaviour
{
    [SerializeField] FortuneWheelManager[] allShop;
    private int[] allShopCost;
    public static SaveAndLoadShop Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DataContainer datalist = DataManager.instance.LoadHargaShop();
        if(datalist!=null)
        {
            for (int i = 0; i < allShop.Length; i++)
            {
                allShop[i].TurnCost = datalist.hargaDiShop[i];
            }
        }
        else
        {
            allShopCost = new int[allShop.Length];
            for (int i = 0; i < allShopCost.Length; i++)
            {
                allShopCost[i] = allShop[i].TurnCost;
            }
            DataManager.instance.SaveData(allShopCost);

        }
    }
    public void Save()
    {
        allShopCost = new int[allShop.Length];
        for (int i = 0; i < allShopCost.Length; i++)
        {
            allShopCost[i] = allShop[i].TurnCost;
        }
        DataManager.instance.SaveData(allShopCost);

    }
}
