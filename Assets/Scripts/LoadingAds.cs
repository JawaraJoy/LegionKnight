using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAds : MonoBehaviour
{
    public GameObject FailedPanel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FailedPanelShow());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FailedPanelShow(){
        yield return new WaitForSeconds(6);
        FailedPanel.SetActive(true);
    }
}
