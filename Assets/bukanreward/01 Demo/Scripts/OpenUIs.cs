using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUIs : MonoBehaviour
{
    public GameObject rewardsUi;
    
    // Start is called before the first frame update
    public void OpenRewardsUI()
    {
        rewardsUi.SetActive(true);
    }
    
}
