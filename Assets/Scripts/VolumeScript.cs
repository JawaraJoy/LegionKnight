using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    [SerializeField] Slider volSlider;
    private float volume = 1;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetFloat("Volume");
        else
            volume = 1;
        volSlider.value = volume;
    }
    public void OnValueChanged()
    {
        FindObjectOfType<GameManager>().ChangeVolume(volSlider.value);
    }
}
