using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Legion Knight/Level")]
    public partial class LevelDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_LevelScene;
        [SerializeField]
        private LevelDefinition m_NextLevel;
        [SerializeField]
        private bool m_IsInfiniteLevel;
        [SerializeField]
        private Sprite m_LevelImage;
        [SerializeField]
        private float m_MinSpeed;
        [SerializeField]
        private float m_MaxSpeed;
        [SerializeField]
        private CurrencyDefinition m_TouchDownRewardCurrency;
        [SerializeField]
        private int m_NormalTouchDownPoint;
        [SerializeField]
        private int m_PerfectTouchDownPoint;
        [SerializeField]
        private List<StandbyPlatformDefinition> m_PlatformAssets = new();

        [SerializeField]
        private BosDefinition m_BosDefinition;
        [SerializeField]
        private AssetReferenceGameObject m_BosAsset;
        [SerializeField]
        private LevelOrnament m_LevelOrnament;
        public BosDefinition BosDefinition => m_BosDefinition;
        public AssetReferenceGameObject BosAsset => m_BosAsset;
        public LevelOrnament LevelOrnament => m_LevelOrnament;
        public string Id => m_Id;
        public string LevelScene => m_LevelScene;
        public Sprite LevelImage => m_LevelImage;
        public LevelDefinition NextLevel => m_NextLevel;
        public bool IsInfiniteLevel => m_IsInfiniteLevel;

        public void StartLevel()
        {
            GameManager.Instance.SetLevelDefinition(this);
            GameManager.Instance.LoadScene(m_LevelScene);
        }

        public float GetSpeed()
        {
            float random = Random.Range(m_MinSpeed, m_MaxSpeed);
            return random;
        }
        public Currency GetNormalTouchDown()
        {
            return new Currency(m_TouchDownRewardCurrency, m_NormalTouchDownPoint);
        }
        public Currency GetPerfectTouchdown()
        {
            return new Currency(m_TouchDownRewardCurrency, m_PerfectTouchDownPoint);
        }
        public int GetNormalTouchDownPoint()
        {
            return m_NormalTouchDownPoint;
        }
        public int GetPerfectTouchDownPoint()
        {
            return m_PerfectTouchDownPoint;
        }
        public List<StandbyPlatformDefinition> GetPlatformAssets()
        {
            return m_PlatformAssets;
        }
        public List<StandbyPlatformDefinition> GetBosPlatformAssets()
        {
            return m_BosDefinition.BosPlatformsAsset;
        }
    }

    [System.Serializable]
    public partial class LevelOrnament
    {
        [SerializeField]
        private Sprite m_BaseGround;
        [SerializeField]
        private Sprite m_LeftCliff;
        [SerializeField]
        private Sprite m_RightCliff;
        [SerializeField]
        private Sprite m_Tree;
        [SerializeField]
        private Sprite m_Mountain;
        [SerializeField]
        private Sprite m_Background;

        public Sprite BaseGround => m_BaseGround;
        public Sprite LeftCliff => m_LeftCliff;
        public Sprite RightCliff => m_RightCliff;
        public Sprite Tree => m_Tree;
        public Sprite Mountain => m_Mountain;
        public Sprite Background => m_Background;
    }
}
