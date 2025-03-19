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
            m_Skills = new List<SkillDefinition>(defi.Passives);
            SpawnImage();
        }
        private void SpawnImage()
        {
            foreach(SkillDefinition skill in m_Skills)
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(m_ImageAsset, m_Content.transform, false);
                OnSpawnIcon(handle, skill);
            }
        }
        private void OnSpawnIcon(AsyncOperationHandle<GameObject> handle, SkillDefinition skill)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out Image image))
                {
                    image.sprite = skill.Icon;
                    m_SkillIcons.Add(image);
                }
            }
        }
    }
}
