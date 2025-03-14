using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class SkillSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<ProjectileSpawn> m_Skills = new();
        public void SpawnProjectile()
        {
            foreach (ProjectileSpawn skill in m_Skills)
            {
                skill.LoadProjectile();
            }
        }
    }
}
