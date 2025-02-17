using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceBuyCharacter : MonoBehaviour
{
    public FortuneWheelManager fortuneWheelManager;
    public Text priceText;
    // Start is called before the first frame update
    private void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        priceText.text = fortuneWheelManager.TurnCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
