using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class GachaDrawResultCard : CollectibleResultItemUI
    {
        [Serializable]
        private sealed class OriginField
        {
            [SerializeField] private TMP_Text m_Label;
            [SerializeField] private Image m_Portrait;
            [SerializeField] private TMP_Text m_Kind;

            public void Bind(CollectibleResultEntry entry)
            {
                var reward = entry.Collectible;
                bool converted = entry.WasDuplicateHero && reward is HeroUnitConfig hero &&
                    hero.ItemDuplicateConverter?.ItemConfig != null;
                m_Label.gameObject.SetActive(converted);
                m_Label.text = converted ? $"Duplicate {reward.BaseInfo.Name}" : string.Empty;
                if (m_Portrait != null)
                {
                    m_Portrait.enabled = converted;
                    m_Portrait.sprite = converted ? reward.CollectibleField.Icon : null;
                }
                if (m_Kind == null) return;
                m_Kind.gameObject.SetActive(!converted);
                m_Kind.text = reward is HeroUnitConfig ? "Hero" : reward is CardConfig ? "Card" : "Item";
            }
        }

        [Serializable]
        private sealed class ArtworkField
        {
            [SerializeField] private Image m_Splash;
            [SerializeField, Range(0.1f, 1f)] private float m_ItemScale = 1f;
            [SerializeField] private Image m_IconFrame;

            public void Bind(CollectibleConfig reward)
            {
                var info = reward.CollectibleField;
                if (m_Splash != null)
                {
                    m_Splash.sprite = info?.SplashImage != null ? info.SplashImage : info?.Icon;
                    m_Splash.enabled = m_Splash.sprite != null;
                    m_Splash.rectTransform.localScale = Vector3.one * (reward is HeroUnitConfig ? 1f : m_ItemScale);
                }
                if (m_IconFrame != null)
                    m_IconFrame.color = info?.RarityConfig != null ? info.RarityConfig.Color : Color.white;
            }
        }

        [SerializeField] private OriginField m_Origin;
        [SerializeField] private ArtworkField m_Artwork;

        protected override bool ShowSingleAmount => true;
        protected override bool TintRarityLabel => false;
        protected override string AmountPrefix => "×";

        protected override void BindOriginInternal(CollectibleResultEntry entry) => m_Origin.Bind(entry);

        protected override void BindFaceInternal(CollectibleConfig reward, int amount)
        {
            base.BindFaceInternal(reward, amount);
            m_Artwork.Bind(reward);
        }
    }
}
