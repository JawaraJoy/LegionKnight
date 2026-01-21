using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CharacterSkillView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_ImageAsset;
        [SerializeField]
        private List<Image> m_SkillIcons = new();

        private List<SkillDefinition> m_Skills = new();

        public void Init(CharacterDefinition defi)
        {
            foreach(Image image in m_SkillIcons)
            {
                Destroy(image.gameObject);
            }
            m_SkillIcons.Clear();
            m_Skills.Clear();
            m_Skills = new List<SkillDefinition>(defi.Passives);
            SpawSkillIcons(m_Skills);
        }
        private void SpawSkillIcons(List<SkillDefinition> definitions)
        {
            foreach (SkillDefinition defi in definitions)
            {
                StartCoroutine(SpawningIcon(defi));
            }
        }
        private IEnumerator SpawningIcon(SkillDefinition defi)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(m_ImageAsset, m_Content.transform, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out Image icon))
                {
                    icon.sprite = defi.Icon;
                    m_SkillIcons.Add(icon);
                }
            }
        }
    }
}
