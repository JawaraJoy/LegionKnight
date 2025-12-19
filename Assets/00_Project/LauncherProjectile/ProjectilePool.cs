using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace LegionKnight
{
    /// <summary>Centralized projectile pooling system.</summary>
    public static class ProjectilePool
    {
        private static readonly Dictionary<ProjectileBase, Stack<ProjectileBase>> Pool = new();

        public static ProjectileBase Get(ProjectileBase prefab, Vector3 pos, Quaternion rot)
        {
            if (!Pool.TryGetValue(prefab, out var stack) || stack.Count == 0)
                return Object.Instantiate(prefab, pos, rot);

            var p = stack.Pop();
            p.transform.SetPositionAndRotation(pos, rot);
            p.gameObject.SetActive(true);
            return p;
        }

        public static void Release(ProjectileBase prefab, ProjectileBase instance)
        {
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(false);

            if (!Pool.ContainsKey(prefab))
                Pool[prefab] = new Stack<ProjectileBase>();

            Pool[prefab].Push(instance);
        }
    }
}