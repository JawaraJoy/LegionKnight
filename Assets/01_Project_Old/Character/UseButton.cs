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
        private TextMeshProUGUI m_UseText;

        private HeroUnit m_HeroUnit;

        private void Start()
        {
            m_UseButton.onClick.RemoveAllListeners();
            m_UseButton.onClick.AddListener(Use);
        }
        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit unit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            m_HeroUnit = unit;
            Refresh();
        }
        private void Refresh()
        {
            bool isCharacterUsed = m_HeroUnit.IsUsed;
            m_UseButton.interactable = !isCharacterUsed;

            m_UseText.text = isCharacterUsed ? "Used" : "Use";
        }
        private void Use()
        {
            Player.Instance.HeroesCollection.SetUsedHero();
            Refresh();
        }
    }
}
