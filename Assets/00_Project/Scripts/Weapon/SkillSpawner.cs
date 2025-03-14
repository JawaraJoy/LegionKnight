using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class SkillSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<ProjectileSpawn> m_Skills = new();

        private void Start()
        {
            SpawnProjectileInternal();
        }
        public void SpawnProjectile()
        {
            //SpawnProjectileInternal();
        }
        private void SpawnProjectileInternal()
        {
            foreach (ProjectileSpawn skill in m_Skills)
            {
                skill.LoadProjectile();
            }
        }
    }
}
