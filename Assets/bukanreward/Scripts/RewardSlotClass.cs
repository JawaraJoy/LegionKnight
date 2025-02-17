using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardSlotClass
{
    public enum PRIORITY { Low, High }
    public PRIORITY priority;
    [SerializeField] private rewardclass item;
    [SerializeField] private int percentage;

    public void add(rewardclass _item)
    {
        item = _item;
        percentage = _item.lowPercentage;
    }
    public RewardSlotClass(rewardclass _item)
    {
        item = _item;
        percentage = _item.lowPercentage;
    }
    public RewardSlotClass()
    {
        item = null;
        percentage = 100;
    }

    public void clear()
    {
        item = null;
        percentage = 100;
    }
    public void changePriority(PRIORITY state)
    {
        priority = state;
        if (priority == PRIORITY.Low)
        {
            percentage = GetRewardclass().lowPercentage;
        }
        else
        {
            percentage = GetRewardclass().highPercentage;
        }
    }
    public rewardclass GetRewardclass() { return item; }
    public int GetAmount() { return percentage; }
}
