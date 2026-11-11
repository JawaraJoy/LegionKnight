using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroSkillView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_ImageAsset;
        [SerializeField]
        private List<Image> m_SkillIcons = new();

        private List<SkillConfig> m_SkillConfigs = new();

        public void Init(HeroUnitConfig heroConfig)
        {
            foreach(Image image in m_SkillIcons)
            {
                Destroy(image.gameObject);
            }
            m_SkillIcons.Clear();
            m_SkillConfigs.Clear();
            m_SkillConfigs = new List<SkillConfig>(heroConfig.Skills);
            SpawSkillIcons(m_SkillConfigs);
        }
        private void SpawSkillIcons(List<SkillConfig> skillConfigs)
        {
            foreach (SkillConfig skillConfig in skillConfigs)
            {
                StartCoroutine(SpawningIcon(skillConfig));
            }
        }
        private IEnumerator SpawningIcon(SkillConfig skillConfig)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(m_ImageAsset, m_Content.transform, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out Image icon))
                {
                    icon.sprite = skillConfig.CollectibleField.Icon;
                    m_SkillIcons.Add(icon);
                }
            }
        }
    }
}
