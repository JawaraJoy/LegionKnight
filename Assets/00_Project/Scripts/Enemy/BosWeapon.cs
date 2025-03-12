using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class BosWeapon : Weapon
    {
        [SerializeField]
        private List<BosAttack> m_BosAttacks = new();

        public void AddBossAttack(BosAttack add)
        {
            m_BosAttacks.Add(add);
        }
        public void RemoveBossAttack(BosAttack add)
        {
            m_BosAttacks.Add(add);
        }
        public void ActiveSkills()
        {
            foreach (BosAttack attack in m_BosAttacks)
            {
                attack.ActiveSkill();
            }
        }
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
        public void AddBossAttack(BosAttack add)
        {
            m_BosWeapon.AddBossAttack(add);
        }
        public void RemoveBossAttack(BosAttack add)
        {
            m_BosWeapon.RemoveBossAttack(add);
        }
        public void ActiveSkills()
        {
            m_BosWeapon.ActiveSkills();
        }
    }
    public partial class GameManager
    {
        public void SetWeaponActive(bool set)
        {
            m_LevelManager.SetWeaponActive(set);
        }
        public void BosShot(int set)
        {
            m_LevelManager.BosShot(set);
        }
        public void AddBossAttack(BosAttack add)
        {
            m_LevelManager.AddBossAttack(add);
        }
        public void RemoveBossAttack(BosAttack add)
        {
            m_LevelManager.AddBossAttack(add);
        }
        public void ActiveSkills()
        {
            m_LevelManager.ActiveSkills();
        }
    }
    public partial class LevelManagerAgent
    {
        public void SetWeaponActive(bool set)
        {
            GameManager.Instance.SetWeaponActive(set);
        }
        public void BosShot(int set)
        {
            GameManager.Instance.BosShot(set);
        }
        public void AddBossAttack(BosAttack add)
        {
            GameManager.Instance.AddBossAttack(add);
        }
        public void RemoveBossAttack(BosAttack add)
        {
            GameManager.Instance.RemoveBossAttack(add);
        }
        public void ActiveSkills()
        {
            GameManager.Instance.ActiveSkills();
        }
    }
}
