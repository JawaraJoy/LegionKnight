using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Reward"),System.Serializable]

public class rewardclass : ScriptableObject
{
    public string nama;
    public int amount;
    public int lowPercentage;
    public int highPercentage;
    public Sprite image;
    public Sprite subImage;
    public enum REWARD_TYPE { Coin,Diamond,Fortune_Wheeler,C_Common,C_Rare,C_Epic}
    public REWARD_TYPE _Type;
}
