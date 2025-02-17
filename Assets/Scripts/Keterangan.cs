using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keterangan : MonoBehaviour
{
    public GameObject SelectedObj, targetObj;

    public GameObject KeteranganCharLocked;
    // Start is called before the first frame update
    void Start()
    {
        SelectedObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectedObs(){
        SelectedObj.SetActive(true);
        SelectedObj.transform.position = targetObj.transform.position;
        SelectedObj.transform.parent = targetObj.transform;
    }

    public void SelectCharLocked(){
        KeteranganCharLocked.SetActive(true);
        StartCoroutine(HideKeterangan());
    }

    IEnumerator HideKeterangan(){
        yield return new WaitForSeconds(2f);
        KeteranganCharLocked.SetActive(false);
    }
}
