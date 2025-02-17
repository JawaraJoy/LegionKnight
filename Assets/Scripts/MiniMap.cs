using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Slider mapSlider;
    [SerializeField] Image playerSliderIcon;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("ModeGamePlay")==0)
            gameObject.SetActive(false);
        mapSlider.value = 0;
        mapSlider.maxValue = FindObjectOfType<GameManager>().ScoreBerapaBossComing;
        RefreshUI();
    }

    private void RefreshUI()
    {
        playerSliderIcon.sprite = SelectedCharacter.Instance.GetPlayer().CharacterHeadIcon;
    }

    private void Update()
    {
        mapSlider.value = FindObjectOfType<GameManager>().ScoreSekarang;
    }
}
