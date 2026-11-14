using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BattleReportCard : InstagramReportCard
    {
        [System.Serializable]
        private sealed class HeroField
        {
            [SerializeField] private TMP_Text m_Name, m_Rarity;
            [SerializeField] private Image m_Portrait, m_Shadow, m_Frame, m_Accent, m_Mark;

            internal void SetHero(UnitConfig hero)
            {
                var art = hero?.CollectibleField;
                var rarity = art?.RarityConfig;
                var portrait = art?.SplashImage != null ? art.SplashImage : art?.Icon;
                m_Portrait.sprite = m_Shadow.sprite = portrait;
                m_Portrait.enabled = m_Shadow.enabled = portrait != null;
                m_Name.text = hero != null ? hero.BaseInfo.Name : "Unknown hero";
                m_Rarity.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
                Color color = rarity != null ? rarity.Color : Color.gray;
                m_Frame.color = m_Accent.color = m_Mark.color = color;
                m_Mark.enabled = rarity != null;
            }
        }

        [System.Serializable]
        private sealed class RunField
        {
            [SerializeField] private TMP_Text m_Mode, m_Wave, m_WaveShadow, m_Score;

            internal void SetResult(StageMode mode, int wave, int score)
            {
                string name = mode == StageMode.Collosal ? "COLOSSAL" : mode.ToString().ToUpperInvariant();
                m_Mode.text = "MODE: " + name;
                m_Wave.text = m_WaveShadow.text = wave.ToString("N0", s_Numbers);
                m_Score.text = score.ToString("N0", s_Numbers);
            }
        }

        [SerializeField] private HeroField m_HeroArt;
        [SerializeField] private RunField m_Run;
        private static readonly CultureInfo s_Numbers = CultureInfo.GetCultureInfo("en-US");

        public Texture2D Capture(UnitConfig hero, StageMode mode, int wave, int score) =>
            CaptureInternal<BattleReportCard>(card =>
            {
                card.m_HeroArt.SetHero(hero);
                card.m_Run.SetResult(mode, wave, score);
            });
    }
}