using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class EvolveDetailsView : MonoBehaviour
    {
        [Serializable]
        private sealed class StarRowField
        {
            [SerializeField] private TMP_Text m_Label;
            [SerializeField] private Image[] m_Stars;
            [SerializeField] private EvolutionStarGradient[] m_Gradients;
            [SerializeField] private Color m_EmptyColor;

            public void Display(int stage)
            {
                m_Label.text = $"Evolve {stage}";
                for (int i = 0; i < m_Stars.Length; i++)
                {
                    int tier = HeroEvolution.StarTier(stage, i);
                    m_Stars[i].color = tier == 0 ? m_EmptyColor : (Color)EvolutionStarGradient.FillColor(tier);
                    m_Gradients[i].enabled = tier == HeroEvolution.MaxStage / HeroEvolution.StarSlots;
                }
            }
        }

        [Serializable]
        private sealed class HeroField
        {
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Rarity;
            [SerializeField] private TMP_Text m_Level;
            [SerializeField] private Image m_Art;
            [SerializeField] private Image m_Rim;
            [SerializeField] private StarRowField m_Current;
            [SerializeField] private StarRowField m_Next;
            [SerializeField] private GameObject m_NextGroup;
            [SerializeField] private GameObject m_Arrow;

            public void Display(HeroUnit unit, int next)
            {
                var config = unit.HeroConfig;
                var art = config.CollectibleField;
                var rarity = art.RarityConfig;
                m_Name.text = config.BaseInfo.Name;
                m_Rarity.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
                m_Rim.color = rarity != null ? rarity.Color : Color.white;
                m_Rarity.color = Color.Lerp(m_Rim.color, Color.white, .52f);
                m_Art.sprite = art.SplashImage != null ? art.SplashImage : art.Icon;
                m_Level.text = $"Level {unit.Level}";
                m_Current.Display(unit.Star);
                m_Next.Display(next);
                m_NextGroup.SetActive(next != unit.Star);
                m_Arrow.SetActive(next != unit.Star);
            }
        }

        [Serializable]
        private sealed class StatFieldView
        {
            [SerializeField] private TMP_Text m_Before;
            [SerializeField] private TMP_Text m_After;
            [SerializeField] private GameObject m_Arrow;

            public void Display(int before, int after, bool max)
            {
                m_Before.text = before.ToString();
                m_After.text = after.ToString();
                m_After.gameObject.SetActive(!max);
                m_Arrow.SetActive(!max);
            }
        }

        [Serializable]
        private sealed class MaterialField
        {
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Count;
            [SerializeField] private TMP_Text m_Status;
            [SerializeField] private Image m_Icon;
            [SerializeField] private Image m_Rim;
            [SerializeField] private Image m_Fill;
            [SerializeField] private Color m_ReadyColor;
            [SerializeField] private Color m_MissingColor;

            public void Display(HeroUnit hero, int owned, bool max)
            {
                var item = hero.EvolveShard;
                int cost = hero.EvolveCost;
                bool ready = hero.Owned && item != null && owned >= cost;
                var color = max || ready ? m_ReadyColor : m_MissingColor;
                m_Name.text = item != null ? item.BaseInfo.Name : "Shard unavailable";
                m_Icon.sprite = item != null ? item.CollectibleField.Icon : null;
                m_Icon.enabled = m_Icon.sprite != null;
                var rarity = item != null ? item.CollectibleField.RarityConfig : null;
                m_Rim.color = rarity != null ? rarity.Color : Color.white;
                m_Count.text = max ? string.Empty : $"{owned} / {cost}";
                m_Count.color = m_Fill.color = m_Status.color = color;
                float progress = max ? 1f : cost > 0 ? Mathf.Clamp01((float)owned / cost) : 0f;
                m_Fill.rectTransform.anchorMax = new Vector2(progress, 1f);
                m_Status.text = max ? "Maximum Evolve reached" : !hero.Owned ? "Recruit this hero first" :
                    item == null ? "Material unavailable" : ready ? "Enough shards" : $"{cost - owned} more shards needed";
            }
        }

        [Serializable]
        private sealed class RewardField
        {
            [SerializeField] private GameObject m_Card;
            [SerializeField] private Image m_Head;
            [SerializeField] private Image m_Frame;
            [SerializeField] private Image m_Rim;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Unlock;

            public void Display(HeroUnit unit, BadgeContent[] badges)
            {
                if (badges != null)
                {
                    foreach (var entry in badges)
                    {
                        if (entry?.Config is not HeroProfileBadgeConfig badge || badge.Hero != unit.HeroConfig) continue;
                        for (int i = 0; i < badge.Upgrade.Length; i++)
                        {
                            var tier = badge.Upgrade[i];
                            if (tier?.RewardOnUnlock == null) continue;
                            foreach (var reward in tier.RewardOnUnlock)
                            {
                                if (reward?.ItemLoot is not CustomImageDefinition frame || frame.Type != CustomImageType.Frame) continue;
                                var art = frame.ProfileAppearance.CollectibleField;
                                m_Name.text = frame.BaseInfo.Name;
                                m_Frame.sprite = art.Icon;
                                m_Head.sprite = unit.HeroConfig.CollectibleField.Icon;
                                m_Rim.color = art.RarityConfig != null ? art.RarityConfig.Color : Color.white;
                                int stage = tier.Target - 1;
                                bool claimed = entry.IsUnlocked && entry.CurrentUpgradeLevel >= i;
                                m_Unlock.text = claimed ? "Claimed from Badge" : unit.Star >= stage ?
                                    "Unlocked · Claim in Badge" : $"After Evolve {stage}, claim in";
                                m_Card.SetActive(true);
                                return;
                            }
                        }
                    }
                }
                m_Card.SetActive(false);
            }
        }

        [Serializable]
        private sealed class ActionField
        {
            [SerializeField] private Button m_Confirm;
            [SerializeField] private Image m_Rim;
            [SerializeField] private TMP_Text m_Label;
            [SerializeField] private GameObject m_Lock;
            [SerializeField] private Color m_ActiveFace;
            [SerializeField] private Color m_DisabledFace;
            [SerializeField] private Color m_ActiveText;
            [SerializeField] private Color m_DisabledText;
            [SerializeField] private Color m_Gold;
            [SerializeField] private Color m_Disabled;

            public void Display(bool ready, bool max)
            {
                m_Confirm.interactable = ready;
                m_Rim.color = ready ? m_Gold : m_Disabled;
                m_Confirm.targetGraphic.color = ready ? m_ActiveFace : m_DisabledFace;
                m_Label.color = ready ? m_ActiveText : m_DisabledText;
                m_Lock.SetActive(!ready && !max);
                m_Label.text = max ? "Max Evolve" : "Evolve";
            }
        }

        [SerializeField] private HeroField m_Hero;
        [SerializeField] private StatFieldView m_Health;
        [SerializeField] private StatFieldView m_Attack;
        [SerializeField] private StatFieldView m_Defense;
        [SerializeField] private MaterialField m_Material;
        [SerializeField] private RewardField m_Reward;
        [SerializeField] private ActionField m_Action;

        public void Display(HeroUnit hero, int owned, BadgeContent[] badges, bool canEvolve)
        {
            int next = Mathf.Min(hero.Star + 1, HeroEvolution.MaxStage);
            bool max = next == hero.Star;
            var config = hero.HeroConfig;
            var current = hero.FinalStat();
            var after = config.MainStats.GetFinalStat(hero.Level) + (config.EvolutionConfig?.GetStatBonus(next) ?? StatField.Zero);
            m_Hero.Display(hero, next);
            m_Health.Display(current.Health, after.Health, max);
            m_Attack.Display(current.Attack, after.Attack, max);
            m_Defense.Display(current.Defense, after.Defense, max);
            m_Material.Display(hero, owned, max);
            m_Reward.Display(hero, badges);
            m_Action.Display(canEvolve, max);
        }
    }
}
