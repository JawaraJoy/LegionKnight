using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Spin Reward", menuName = "Legion Knight/SpinWheel/SpinReward")]
    public class SpinRewardDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private Color m_FrameColor = Color.white;
        [SerializeField]
        private LootDefinition m_Rewards;
        public string Id => m_Id;
        public Color FrameColor => m_FrameColor;
        public LootDefinition Rewards => m_Rewards;

        

    }
}
