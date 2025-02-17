using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitch : MonoBehaviour
{
    [SerializeField] Button indBTN, engBTN;
    private void Start()
    {
        indBTN.onClick.AddListener(() => LanguageManager.instance.SetLanguage(1));
        engBTN.onClick.AddListener(() => LanguageManager.instance.SetLanguage(0));

        if (LanguageManager.instance.GetLanguageIndex() == 0)
        {
            indBTN.interactable = true;
            engBTN.interactable = false;
        }
        else if (LanguageManager.instance.GetLanguageIndex() == 1)
        {
            indBTN.interactable = false;
            engBTN.interactable = true;
        }
    }

    private void OnEnable()
    {
        LanguageManager.instance.onLanguageChange += OnLanguageChange;
    }

    private void OnLanguageChange(int _languangeIndex)
    {
        if (_languangeIndex == 0)
        {
            indBTN.interactable = true;
            engBTN.interactable = false;
        }
        else if (_languangeIndex == 1)
        {
            indBTN.interactable = false;
            engBTN.interactable = true;
        }
    }
}
