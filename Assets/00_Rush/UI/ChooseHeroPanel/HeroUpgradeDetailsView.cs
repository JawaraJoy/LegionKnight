using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HeroUpgradeDetailsView : MonoBehaviour
    {
        [Serializable]
        private sealed class StatRow
        {
            [SerializeField] private TMP_Text m_Current;
            [SerializeField] private TMP_Text m_Next;
            [SerializeField] private GameObject m_Arrow;

            public void Display(int current, int next, bool max)
            {
                m_Current.text = current.ToString();
                m_Next.text = max ? string.Empty : next.ToString();
                m_Arrow.SetActive(!max);
            }
        }

        [Header("Hero")]
        [SerializeField] private TMP_Text m_HeroName, m_Rarity, m_CurrentLevel, m_NextLevel, m_Cap;
        [SerializeField] private Image m_Art, m_HeroRim;
        [SerializeField] private GameObject m_NextCaption, m_LevelArrow;

        [Header("Cost")]
        [SerializeField] private TMP_Text m_Currency, m_Count, m_Status;
        [SerializeField] private Image m_CoinIcon, m_CoinRim, m_Fill;
        [SerializeField] private Color m_ReadyInk, m_MissingInk, m_ReadyFill, m_MissingFill;

        [Header("Stats")]
        [SerializeField] private TMP_Text m_BeforeHeading, m_AfterHeading;
        [SerializeField] private StatRow m_Health, m_Attack, m_Defense;

        [Header("Action")]
        [SerializeField] private Image m_Face, m_Rim;
        [SerializeField] private TMP_Text m_Label;
        [SerializeField] private GameObject m_Lock;
        [SerializeField] private Color m_ReadyFace, m_DisabledFace, m_ReadyRim, m_DisabledRim, m_ReadyText, m_DisabledText;

        public void Display(HeroUnit hero, ItemConfig item, int cost, int owned, bool ready, bool max)
        {
            var config = hero.HeroConfig;
            var art = config.CollectibleField;
            var rarity = art.RarityConfig;
            int level = hero.Level;
            m_HeroName.text = config.BaseInfo.Name;
            m_Art.sprite = art.SplashImage != null ? art.SplashImage : art.Icon;
            m_HeroRim.color = rarity != null ? rarity.Color : Color.white;
            m_Rarity.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
            m_Rarity.color = Color.Lerp(m_HeroRim.color, Color.white, .52f);
            m_CurrentLevel.text = $"Lv. {level}";
            m_NextLevel.text = max ? string.Empty : $"Lv. {level + 1}";
            m_LevelArrow.SetActive(!max);
            m_NextCaption.SetActive(!max);
            m_Cap.text = $"Level cap  {HeroEvolution.MaxLevel}";

            string currency = item != null ? item.BaseInfo.Name : string.Empty;
            var coinRarity = item != null ? item.CollectibleField.RarityConfig : null;
            m_Currency.text = currency;
            m_CoinIcon.sprite = item != null ? item.CollectibleField.Icon : null;
            m_CoinIcon.enabled = m_CoinIcon.sprite != null;
            m_CoinRim.color = coinRarity != null ? coinRarity.Color : Color.white;
            m_Count.text = max ? string.Empty : item != null ? $"{owned:N0} / {cost:N0}" : "—";
            m_Status.text = max ? "Maximum level reached" : !hero.Owned ? "Recruit this hero first" :
                item == null || cost <= 0 ? "Cost unavailable" : ready ? $"Enough {currency}" : $"{cost - owned:N0} more {currency} needed";
            m_Status.color = m_Count.color = max || ready ? m_ReadyInk : m_MissingInk;
            m_Fill.color = max || ready ? m_ReadyFill : m_MissingFill;
            float fill = max ? 1f : cost > 0 ? Mathf.Clamp01((float)owned / cost) : 0f;
            m_Fill.rectTransform.anchorMax = new Vector2(fill, 1f);

            m_BeforeHeading.text = $"Level {level}";
            m_AfterHeading.text = max ? string.Empty : $"Level {level + 1}";
            var before = hero.FinalStat();
            var after = max ? before : hero.NextFinalStat();
            m_Health.Display(before.Health, after.Health, max);
            m_Attack.Display(before.Attack, after.Attack, max);
            m_Defense.Display(before.Defense, after.Defense, max);

            m_Label.text = max ? "Max Level" : "Upgrade";
            m_Lock.SetActive(!ready && !max);
            m_Rim.color = ready ? m_ReadyRim : m_DisabledRim;
            m_Face.color = ready ? m_ReadyFace : m_DisabledFace;
            m_Label.color = ready ? m_ReadyText : m_DisabledText;
        }
    }
}
