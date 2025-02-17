using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGameOver : MonoBehaviour
{
    public GameObject[] GameoverPanelRandom;

    public GameObject PanelRevive, PanelReward;
    // Start is called before the first frame update

    public void RandomPanelGameOver()
    {
        int i = Random.Range(0, GameoverPanelRandom.Length);
        GameoverPanelRandom[i].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HidePanelReviveReward(){
        PanelRevive.SetActive(false);
        PanelReward.SetActive(false);
    }
}
