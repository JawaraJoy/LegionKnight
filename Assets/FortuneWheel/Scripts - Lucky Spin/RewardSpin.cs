using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardSpin", menuName = "RewardSpin/RewardObject", order = 1)]
public class RewardSpin : ScriptableObject
{
    public RewardType rewardType;
    public CharacterScriptableObj character;
    public int amount;
    public Sprite rewardImage;
    public Rarity rewardRarity;
    public string rewardName;
}

public enum RewardType
{
    coin,diamond,character,key
}

public enum Rarity
{
    coin,diamond,common,rare,epic,legendary
}
