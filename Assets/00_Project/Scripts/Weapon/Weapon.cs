using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class WeaponForm
    {
        [SerializeField]
        private List<ProjectileSpawn> m_ProjectileSpawners = new();
        public void SpawnProjectile()
        {
            foreach (ProjectileSpawn ps in m_ProjectileSpawners)
            {
                ps.LoadProjectile();
            }
        }
    }
    public partial class Weapon : MonoBehaviour
    {
        private int m_WeaponIndex;
        private bool m_WeaponActive;
        [SerializeField]
        private List<WeaponForm> m_WeaponForms = new();
        public void SetWeaponActive(bool set)
        {
            m_WeaponActive = set;
        }
        public void Shot()
        {
            if (!m_WeaponActive) return;
            ClampWeaponIndex();
            m_WeaponForms[m_WeaponIndex].SpawnProjectile();
        }
        private void ClampWeaponIndex()
        {
            m_WeaponIndex = Mathf.Clamp(m_WeaponIndex, 0, m_WeaponForms.Count);
        }
        public void SetWeaponIndex(int set)
        {
            m_WeaponIndex = set - 1;
            ClampWeaponIndex();
        }
    }
}
