using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePicture : MonoBehaviour
{
    Image PP;
    public void SetPP(Sprite newPP) { PP.sprite = newPP; }

    public static ProfilePicture Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        PP = GetComponent<Image>();
    }

}
