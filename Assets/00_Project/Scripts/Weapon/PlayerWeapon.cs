using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerWeapon : Weapon
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerWeapon m_PlayerWeapon;

        public void Shot()
        {
            m_PlayerWeapon.Shot();
        }
        public void SetWeaponIndex(int set)
        {
            m_PlayerWeapon.SetWeaponIndex(set);
        }
        public void SetWeaponActive(bool set)
        {
            m_PlayerWeapon.SetWeaponActive(set);
        }
    }
    public partial class PlayerAgent
    {
        public void Shot()
        {
            Player.Instance.Shot();
        }
        public void SetWeaponIndex(int set)
        {
            Player.Instance.SetWeaponIndex(set);
        }
        public void SetWeaponActive(bool set)
        {
            Player.Instance.SetWeaponActive(set);
        }
    }
}
