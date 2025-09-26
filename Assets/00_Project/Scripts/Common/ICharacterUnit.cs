using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public interface ICharacterUnit
    {
        string CharacterName { get; }
        int CurrentMaxExp { get; }
        CharacterDefinition Definition { get; }
        int Exp { get; }
        Sprite Icon { get; }
        bool IsUsed { get; }
        int Level { get; }
        int MaxLevel { get; }
        int MaxStar { get; }
        bool Owned { get; }
        List<SkillDefinition> Passives { get; }
        CurrencyDefinition ShardDefinition { get; }
        Sprite SmallIcon { get; }
        int Star { get; }
        StandbyPlatformDefinition UniquePlatform { get; }
        List<SkillDefinition> Weapons { get; }

        void AddExp(int exp);
        void AddLevel(int level);
        void AddStar(int add);
        bool CanBreak();
        Stat FinalStat();
        Currency GetBreakCoinCost();
        Currency GetBreakCost();
        void Init();
        void LevelUp();
        Stat NextFinalStat();
        void SetExp(int exp);
        void SetIsUsed(bool isUsed);
        void SetOwned(bool set);
        void SetOwner(Object owner);
    }
}