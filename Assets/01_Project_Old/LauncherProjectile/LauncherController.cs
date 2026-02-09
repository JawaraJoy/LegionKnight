using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private LauncherConfig m_Config;
        [SerializeField] private Transform m_FirePoint;


        public void Fire(Transform target = null)
        {
            if (m_Config.RequireTarget && target == null)
                return;


            StartCoroutine(FireRoutine(target));
        }


        private IEnumerator FireRoutine(Transform target)
        {
            foreach (var shot in m_Config.Projectiles)
            {
                SpawnProjectile(shot, target);


                if (m_Config.FireInterval > 0f)
                    yield return new WaitForSeconds(m_Config.FireInterval);
            }
        }


        private void SpawnProjectile(ProjectileShotConfig shot, Transform target)
        {
            Quaternion rotation;

            if (shot.UseLauncherRotation)
            {
                rotation = m_FirePoint.rotation *
                           Quaternion.FromToRotation(Vector3.right, shot.LocalDirection.normalized);
            }
            else
            {
                rotation = Quaternion.FromToRotation(Vector3.right, shot.LocalDirection.normalized);
            }

            ProjectileBase projectile = ProjectilePool.Get(
                shot.ProjectilePrefab,
                m_FirePoint.position,
                rotation
            );

            projectile.Initialize(shot, target);
        }
    }
}
