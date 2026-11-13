using System;
using LegionKnight;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class GachaHeroRevealPanel : PanelView
    {
        [SerializeField] private AvatarSpineUI m_AvatarSpineUI;
        [SerializeField] private TextMeshProUGUI m_HeroNameText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private AvatarSpineUI m_AuraEffectspine;
        [SerializeField] private AnimationClipConfig m_RevealClip;
        [SerializeField] private AnimationClipConfig m_EffectClip;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Button m_SkipButton;

        [Serializable]
        private struct StandingPoseField
        {
            [SerializeField] private SkeletonDataAsset m_Rig;
            [Tooltip("Midpoint of the planted feet in the idle pose, in Spine skeleton units.")]
            [SerializeField] private Vector2 m_Ground;
            [Tooltip("Height from the soles to the crown in the idle pose, in Spine skeleton units.")]
            [SerializeField] private float m_Height;

            public SkeletonDataAsset Rig => m_Rig;
            public Vector2 Ground => m_Ground;
            public float Height => m_Height;
        }

        [Serializable]
        private sealed class PresentationField
        {
            private const string c_StraightAlphaKeyword = "_STRAIGHT_ALPHA_INPUT";
            private const float c_AuraTintBlend = 0.82f;
            private const float c_StandingHeight = 4.1f;
            [SerializeField] private SkeletonGraphic m_HeroGraphic;
            [SerializeField] private StandingPoseField[] m_StandingPoses = Array.Empty<StandingPoseField>();
            [SerializeField] private Material m_PremultipliedHeroMaterial;
            [SerializeField] private Material m_StraightHeroMaterial;
            [SerializeField] private Color m_AuraTint;
            [SerializeField] private TextMeshProUGUI m_Banner;
            [SerializeField] private TextMeshProUGUI m_Epithet;
            [SerializeField] private Image m_RarityMark;

            public void Bind(HeroUnitConfig hero, GachaBannerConfig banner, RarityConfig rarity)
            {
                var atlasMaterial = hero.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
                m_HeroGraphic.material = atlasMaterial.IsKeywordEnabled(c_StraightAlphaKeyword)
                    ? m_StraightHeroMaterial : m_PremultipliedHeroMaterial;
                m_Banner.text = banner != null ? banner.BaseInfo.Name : string.Empty;
                m_Epithet.text = string.Empty;
                var manager = RushPlayer.Instance.GachaManager;
                foreach (var candidate in manager.Banners)
                {
                    if (candidate == null || GachaRateInfo.SingleReward(candidate.FinalPityGuarantees) != hero) continue;
                    m_Epithet.text = candidate.PanelEpithet;
                    break;
                }
                m_RarityMark.enabled = rarity != null;
                if (rarity != null) m_RarityMark.color = rarity.Color;
            }

            public Color AuraColor(Color rarityColor)
            {
                var tint = Color.Lerp(rarityColor, m_AuraTint, c_AuraTintBlend);
                tint.a = m_AuraTint.a;
                return tint;
            }

            public void AlignStandingPose()
            {
                var ground = Vector2.zero;
                float scale = 1f;
                foreach (var pose in m_StandingPoses)
                {
                    if (pose.Rig != m_HeroGraphic.SkeletonDataAsset || pose.Height <= 0f) continue;
                    ground = pose.Ground;
                    scale = c_StandingHeight / pose.Height;
                    break;
                }

                // Keep the graphic's pivot on the aura while the entrance tween and idle run.
                var skeleton = m_HeroGraphic.Skeleton;
                skeleton.SetScale(scale, scale);
                skeleton.SetPosition(-ground.x * scale, -ground.y * scale);
                skeleton.UpdateWorldTransform(Spine.Physics.Pose);
                m_HeroGraphic.UpdateMesh();
            }
        }

        [SerializeField] private PresentationField m_Presentation;
        private Action m_OnClosed;
        private Action m_OnSkip;

        public void Show(HeroUnitConfig hero, Action onClosed) =>
            OpenInternal(hero, RushPlayer.Instance.GachaManager.ActiveBanner, onClosed, null);

        internal void Show(HeroUnitConfig hero, GachaBannerConfig banner, Action onClosed, Action onSkip) =>
            OpenInternal(hero, banner, onClosed, onSkip);

        private void OpenInternal(HeroUnitConfig hero, GachaBannerConfig banner, Action onClosed, Action onSkip)
        {
            if (hero == null) return;
            ShowInternal();
            if (!IsShown) return;
            m_OnClosed = onClosed;
            m_OnSkip = onSkip;
            m_SkipButton.gameObject.SetActive(onSkip != null);
            m_SkipButton.onClick.RemoveListener(OnSkipClickedInternal);
            if (onSkip != null) m_SkipButton.onClick.AddListener(OnSkipClickedInternal);
            m_CloseButton.onClick.RemoveListener(Hide);
            m_CloseButton.onClick.AddListener(Hide);

            var rarity = hero.CollectibleField?.RarityConfig;
            m_HeroNameText.text = hero.BaseInfo.Name;
            m_RarityText.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
            var color = rarity != null ? rarity.Color : Color.white;
            m_RarityFrame.color = color;
            m_Presentation.Bind(hero, banner, rarity);

            m_AvatarSpineUI.SetSkeletonDataAsset(hero.SkeletonDataAsset);
            m_AvatarSpineUI.PlayClipInterrupt(m_RevealClip);
            m_Presentation.AlignStandingPose();
            m_AuraEffectspine.SetColor(m_Presentation.AuraColor(color));
            m_AuraEffectspine.PlayClipInterrupt(m_EffectClip);
        }

        private void OnSkipClickedInternal() => m_OnSkip?.Invoke();

        protected override void HideInternal()
        {
            m_CloseButton.onClick.RemoveListener(Hide);
            m_SkipButton.onClick.RemoveListener(OnSkipClickedInternal);
            m_OnSkip = null;
            var closed = m_OnClosed;
            m_OnClosed = null;
            base.HideInternal();
            closed?.Invoke();
        }
    }
}
