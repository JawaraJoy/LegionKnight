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
        [SerializeField]
        private int m_MaxWeaponIndex;
        private int m_WeaponIndex;
        private bool m_WeaponActive;
        [SerializeField]
        private List<WeaponForm> m_WeaponForms = new();

        private void Start()
        {
            m_MaxWeaponIndex = m_WeaponForms.Count;
        }
        public void SetWeaponActive(bool set)
        {
            m_WeaponActive = set;
        }
        public void Shot(int set)
        {
            if (!m_WeaponActive) return;
            SetWeaponIndexInternal(set);
            m_WeaponForms[m_WeaponIndex].SpawnProjectile();
        }
        private void ClampWeaponIndex()
        {
            m_WeaponIndex = Mathf.Clamp(m_WeaponIndex, 0, m_WeaponForms.Count - 1);
        }
        public void SetWeaponIndexInternal(int set)
        {
            m_WeaponIndex = set;
            ClampWeaponIndex();
        }
        public void SetWeaponIndex(int set)
        {
            SetWeaponIndexInternal(set);
        }
        public void AddWeaponIndex(int add)
        {
            m_WeaponIndex += add;
            ClampWeaponIndex();
        }
    }
}
