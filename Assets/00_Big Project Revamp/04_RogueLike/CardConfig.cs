using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Rush/RogueLike/Card", order = 2)]
    public class CardConfig : CollectibleConfig
    {
        [SerializeField]
        private SkillConfig[] m_SkillConfigs;

        public SkillConfig[] SkillConfigs => m_SkillConfigs;

        public void Collect()
        {
            Unit player = RushPlayer.Instance.Unit;
            if (player.HasBind(out SkillController skillController))
            {
                skillController.ForceActives(SkillConfigs);

                RogueLikeManager manager = RushGameManager.Instance.RogueLikeManager;
                manager.OnCardSelected?.Invoke(this);
            }
        }
    }
}
