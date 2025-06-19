using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SocialPlatforms;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ProjectileSpawn
    {
        [SerializeField]
        private Vector2 m_StartingForce;
        [SerializeField]
        private Vector3 m_StartingRotation;
        [SerializeField]
        private bool m_Local = false;
        [SerializeField]
        private AssetReferenceGameObject m_ProjectileAsset;
        private AsyncOperationHandle<GameObject> m_Handle;

        [SerializeField]
        private Transform m_SpawnPost;

        private ProjectileDamage m_SpawnedProjectileDamage;

        [SerializeField]
        private UnityEvent<ProjectileDamage> m_OnWeaponSpawned = new();

        private AbilityDefinition m_AbilityDefinition;

        public void LoadProjectile(AbilityDefinition ability)
        {
            m_AbilityDefinition = ability;
            if (m_Local)
            {
                m_Handle = m_ProjectileAsset.InstantiateAsync(m_SpawnPost, false);
            }
            else
            {
                m_Handle = m_ProjectileAsset.InstantiateAsync(m_SpawnPost.position, Quaternion.identity);
            }
            m_Handle.Completed += SpawnProjectile;
        }

        private void OnWeaponSpawnedInvoke()
        {
            m_OnWeaponSpawned?.Invoke(m_SpawnedProjectileDamage);
        }

        private void SpawnProjectile(AsyncOperationHandle<GameObject> handle)
        {
            CharacterDefinition characterDefi = Player.Instance.UsedCharacter;
            CharacterUnit usedChar = Player.Instance.GetCharacterUnit(characterDefi);
            int level = usedChar.Level;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out IAbility ability))
                {
                    ability.Initialize(m_AbilityDefinition, level);
                }
                if (result.TryGetComponent(out ISelfAbility selfAbility))
                {
                    selfAbility.Initialize();
                }
                if (result.TryGetComponent(out ProjectileDamage projectile))
                {
                    m_SpawnedProjectileDamage = projectile;
                    m_SpawnedProjectileDamage.transform.rotation = Quaternion.Euler(m_StartingRotation);
                    m_SpawnedProjectileDamage.AddForce(m_StartingForce);
                    m_SpawnedProjectileDamage.FindTarget();
                    OnWeaponSpawnedInvoke();
                }
            }
        }
    }
}
