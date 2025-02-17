using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activewhenrewardsready : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        //disable when Rewards ads are not ready
        /*if (BspLabsAdsMobile.Instance.IsRewardVideoAvailable() && gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }*/
    }
}
