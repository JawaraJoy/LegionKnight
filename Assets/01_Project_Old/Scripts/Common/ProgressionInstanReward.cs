using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New ProgressionInstanReward", menuName = "Legion Knight/Progression Instan Reward", order = 1)]
    public partial class ProgressionInstanReward : ScriptableObject
    {
        [SerializeField]
        private int m_Amount;

        public void AddExp()
        {
            Player.Instance.AddPlayerExperience(m_Amount);
        }
    }
}
