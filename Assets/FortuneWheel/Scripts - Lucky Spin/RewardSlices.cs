using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RewardSlices : MonoBehaviour
{
    public Image theRewardImageUI;
    public Text rewardTextUI;
    public RewardSpin theReward;
    public float degree;

    public void RefreshReward()
    {
        theRewardImageUI.sprite = theReward.rewardImage;
        if (theReward.rewardType == RewardType.coin || theReward.rewardType == RewardType.diamond)
            rewardTextUI.text = theReward.amount.ToString();
        else if (theReward.rewardType == RewardType.character)
            rewardTextUI.text = theReward.rewardName;

    }
}
