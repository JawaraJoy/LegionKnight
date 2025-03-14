using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Legion Knight/Bos Enemy")]
    public partial class BosDefinition : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_Health;
        [SerializeField]
        private List<StanbyPlatform> m_BosPlatforms = new();

        public Sprite Icon => m_Icon;
        public int Health => m_Health;
        public List<StanbyPlatform> BosPlatformsAsset => m_BosPlatforms;
    }
    public partial class BosEnemy
    {
        [SerializeField]
        private BosDefinition m_BosDefinition;
        private Sprite IconInternal => m_BosDefinition.Icon;
        private int HealthInternal => m_BosDefinition.Health;
        private List<StanbyPlatform> BosPlatformsInternal => m_BosDefinition.BosPlatformsAsset;

        [SerializeField]
        private UnityEvent<BosDefinition> m_OnSetBosDefinition = new();

        public void SetBosDefinition(BosDefinition definition)
        {
            m_BosDefinition = definition;
            OnSetBosDefinitionInvoke(definition);
        }

        private void OnSetBosDefinitionInvoke(BosDefinition definition)
        {
            m_OnSetBosDefinition?.Invoke(definition);
        }
    }
}
