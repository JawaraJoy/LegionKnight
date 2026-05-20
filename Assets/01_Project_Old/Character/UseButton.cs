using MoreMountains.Tools;
using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UseButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_UseButton;
        [SerializeField]
        private Button m_WatchToUTryOnceButton;

        [SerializeField]
        private TextMeshProUGUI m_UseText;
        [SerializeField]
        private TextMeshProUGUI m_WatchToTryOnceText;
        [SerializeField, MMReadOnly]
        private HeroUnit m_HeroUnit;

        private void Start()
        {
            m_UseButton.onClick.AddListener(Use);
            m_WatchToUTryOnceButton.onClick.AddListener(WatchToTryOnce);
            UnityService.Instance.LoadRewardedAd();
        }
        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit unit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            m_HeroUnit = unit;
            Refresh();
        }
        private void Refresh()
        {
            bool isHeroUnlocked = m_HeroUnit.Owned;
            bool isCharacterUsed = m_HeroUnit.IsUsed;
            bool isOnTrial = m_HeroUnit.OnTrial;
            m_UseButton.interactable = !isCharacterUsed && isHeroUnlocked;

            m_UseText.text = isCharacterUsed ? "Used" : "Use";
            m_WatchToUTryOnceButton.gameObject.SetActive(!isHeroUnlocked);
            m_WatchToTryOnceText.text = !isOnTrial ? "Watch to Try" : "Used";
            m_WatchToUTryOnceButton.interactable = !isOnTrial;
        }
        private void Use()
        {
            Player.Instance.HeroesCollection.SetUsedHero();
            Refresh();
        }
        private void WatchToTryOnce()
        {
            UnityService.Instance.ShowRewardedAd(UnlockHero);
        }

        private void UnlockHero()
        {
            m_HeroUnit.SetTrial(true);
            Use();
        }
    }
}
