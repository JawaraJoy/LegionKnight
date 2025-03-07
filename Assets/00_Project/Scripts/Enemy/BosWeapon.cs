using UnityEngine;

namespace LegionKnight
{
    public partial class BosWeapon : Weapon
    {
        
    }

    public partial class BosEnemy
    {
        [SerializeField]
        private BosWeapon m_BosWeapon;

        public void SetWeaponActive(bool set)
        {
            m_BosWeapon.SetWeaponActive(set);
        }
        public void Shot(int set)
        {
            m_BosWeapon.Shot(set);
        }
    }
    public partial class GameManager
    {
        public void SetWeaponActive(bool set)
        {
            m_LevelManager.SetWeaponActive(set);
        }
        public void Shot(int set)
        {
            m_LevelManager.Shot(set);
        }
    }
    public partial class LevelManagerAgent
    {
        public void SetWeaponActive(bool set)
        {
            GameManager.Instance.SetWeaponActive(set);
        }
        public void Shot(int set)
        {
            GameManager.Instance.Shot(set);
        }
    }
}
