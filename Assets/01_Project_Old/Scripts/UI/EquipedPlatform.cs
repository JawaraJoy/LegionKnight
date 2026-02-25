using MoreMountains.Tools;
using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class EquipedPlatform : UIView
    {
        [SerializeField]
        private Image m_EquipedIcon;
        [SerializeField]
        private UnityEvent<PlatformUnit> m_OnPlatformEquiped;

        [SerializeField, MMReadOnly]
        private PlatformConfig m_SelectedPlatformConfig;
        public void SetSelected(PlatformConfig platformConfig)
        {
            m_SelectedPlatformConfig = platformConfig;
        }
        public void Equip()
        {
            PlatformUnit unit = Player.Instance.PlatformDeck.GetPlatformOwned(m_SelectedPlatformConfig);
            Player.Instance.PlatformDeck.SetIsEquiped(m_SelectedPlatformConfig, true);
            m_OnPlatformEquiped.Invoke(unit);
            m_EquipedIcon.sprite = m_SelectedPlatformConfig.CollectibleField.Icon;
        }
        public void Init()
        {
            PlatformConfig equiped = Player.Instance.PlatformDeck.GetUsedStanbyPlatform();
            bool isOwned = Player.Instance.PlatformDeck.IsPlatformOwned(equiped);
            if (isOwned)
            {
                ShowInternal();
                m_EquipedIcon.sprite = equiped.CollectibleField.Icon;
            }
            else
            {
                HideInternal();
            }
        }
    }
}
