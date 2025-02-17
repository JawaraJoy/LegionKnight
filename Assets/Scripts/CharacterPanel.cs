using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public Text activityPointText;
    public Slider activityPointSlider;
    // Start is called before the first frame update
    void OnEnable()
    {
        ActivityPoint.Instance.onActivitypointChange += onActivitypointChange;
    }
    private void OnDisable()
    {
        ActivityPoint.Instance.onActivitypointChange -= onActivitypointChange;
    }

    private void onActivitypointChange(int activityPoint)
    {
        activityPointSlider.value = activityPoint;
        activityPointText.text = $"Activity Point : {activityPoint}%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
