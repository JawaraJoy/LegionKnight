using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    private int countdownValue = 7;
    public AdsManager AdsObject;
    void Start()
    {
        if (countdownText == null)
        {
            Debug.LogError("TextMeshProUGUI component is not assigned.");
            return;
        }
        
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownValue >= 0)
        {
            countdownText.text = countdownValue.ToString();
            yield return new WaitForSeconds(1);
            countdownValue--;
        }
        AdsObject.GetComponent<AdsManager>().ShowRewarded();
        Debug.Log("Countdown reached zero!");
    }
}