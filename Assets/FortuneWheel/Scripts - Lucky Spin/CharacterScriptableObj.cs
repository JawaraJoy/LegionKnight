using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptTable", menuName = "Character/CharacterScriptTable", order = 1)]
public class CharacterScriptableObj : ScriptableObject
{
    public string CharacterID;
    public string CharacterName;
    public GameObject CharacterPrefab;

    public Sprite CharacterIcon;
    public Sprite CharacterHeadIcon;
    public Rarity CharacterRarity;
}
