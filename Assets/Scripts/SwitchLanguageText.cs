using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLanguageText : MonoBehaviour
{
    [SerializeField] SwitchLanguageTextBlueprint[] allText;
    private void Start()
    {
        foreach (var item in allText)
        {
            item.teksAwal = item.tekstKomponen.text;
            if(LanguageManager.instance.GetLanguageIndex() == 0)
            {
                item.tekstKomponen.text = item.teksAwal;
            }
            else if(LanguageManager.instance.GetLanguageIndex() == 1)
            {
                item.tekstKomponen.text = item.terjemahan;
            }
        }
        LanguageManager.instance.onLanguageChange += LanguageChange;
    }
    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        LanguageManager.instance.onLanguageChange -= LanguageChange;
    }
    private void LanguageChange(int _languangeIndex)
    {
        foreach (var item in allText)
        {
            if (_languangeIndex == 0)
            {
                item.tekstKomponen.text = item.teksAwal;
            }
            if(_languangeIndex == 1)
            {
                item.tekstKomponen.text = item.terjemahan;
            }
        }
    }

}
