using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSSetting : MonoBehaviour
{
    [SerializeField] Toggle fpsA,fpsB;

    private void Start()
    {
        fpsA.onValueChanged.AddListener(OnToogleAValuedChange);
        fpsB.onValueChanged.AddListener(OnToogleBValuedChange);
        if(PlayerPrefs.HasKey("FPS"))
        {
            fpsB.isOn = PlayerPrefs.GetInt("FPS") == 60 ? true:false;
            if(fpsB.isOn)
            {
                fpsB.interactable = false;
                fpsA.interactable = true;
                fpsA.isOn = false;
                Application.targetFrameRate = 60;
                PlayerPrefs.SetInt("FPS", 60);
            }
            else
            {
                fpsA.isOn = true;
                fpsA.interactable = false;
                fpsB.interactable = true;
                Application.targetFrameRate = 30;
                PlayerPrefs.SetInt("FPS", 30);
            }
        }
        else
        {
            fpsA.isOn = true;
            fpsB.isOn = false;
            fpsA.interactable = false;
            Application.targetFrameRate = 30;
            PlayerPrefs.SetInt("FPS", 30);
        }
    }

    private void OnToogleBValuedChange(bool isOn)
    {
        if(isOn)
        {
            fpsA.isOn = false;
            fpsB.interactable = false;
            fpsA.interactable = true;
            Application.targetFrameRate = 60;
            PlayerPrefs.SetInt("FPS", 60);
        }
    }

    private void OnToogleAValuedChange(bool isOn)
    {
        if(isOn)
        {
            fpsB.isOn = false;
            fpsA.interactable = false;
            fpsB.interactable = true;
            Application.targetFrameRate = 30;
            PlayerPrefs.SetInt("FPS", 30);
        }
    }
} 
