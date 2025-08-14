using MoreMountains.Tools;
using Newtonsoft.Json;
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
        private StandbyPlatformDefinition m_Selected;
        public void SetSelected(StandbyPlatformDefinition select)
        {
            m_Selected = select;
        }
        public void Equip()
        {
            PlatformUnit unit = Player.Instance.GetPlatformOwned(m_Selected);
            Player.Instance.SetPlatformUnitIsEquiped(m_Selected, true);
            m_OnPlatformEquiped.Invoke(unit);
            m_EquipedIcon.sprite = m_Selected.Icon;
        }
        public void Init()
        {
            StandbyPlatformDefinition equiped = Player.Instance.GetUsedStanbyPlatform();
            bool isOwned = Player.Instance.IsPlatformOwned(equiped);
            if (isOwned)
            {
                ShowInternal();
                m_EquipedIcon.sprite = equiped.Icon;
            }
            else
            {
                HideInternal();
            }
        }
    }
}
