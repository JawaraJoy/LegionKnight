using UnityEngine;

namespace Rush
{
    public class UnitConfigExtension_SpineAnimationClip
    {
        
    }

    public partial class UnitConfig
    {
        [Header("Animation")]
        [SerializeField]
        private AnimationClipConfig m_Idle;
        [SerializeField]
        private AnimationClipConfig m_Attack;
        [SerializeField]
        private AnimationClipConfig m_PerfectAttack;
        [SerializeField]
        private AnimationClipConfig m_Hurt;
        [SerializeField]
        private AnimationClipConfig m_Death;
        [SerializeField]
        private AnimationClipConfig m_Jump;
        [SerializeField]
        private AnimationClipConfig m_Emote;

        public AnimationClipConfig Idle => m_Idle;

        public void PlayIdle(Unit unit)
        {
             Play(unit, unit.Config.m_Idle);
        }
        public void PlayAttack(Unit unit)
        {
            Play(unit, unit.Config.m_Attack);
        }
        public void PlayPerfectAttack(Unit unit)
        {
            Play(unit, unit.Config.m_PerfectAttack);
        }
        public void PlayHurt(Unit unit)
        {
            Play(unit, unit.Config.m_Hurt);
        }
        public void PlayDeath(Unit unit)
        {
            Play(unit, unit.Config.m_Death);
        }
        public void PlayJump(Unit unit)
        {
            Play(unit, unit.Config.m_Jump);
        }
        public void PlayEmote(Unit unit)
        {
            Play(unit, unit.Config.m_Emote);
        }
        private void Play(Unit unit, AnimationClipConfig config) 
        {
            if (unit.HasBind(out AvatarSpine avatarSpine))
            {
                avatarSpine.PlayClipInterrupt(config);
            }
            if (unit.HasBind(out AvatarSpineUI avatarSpineUI))
            {
                avatarSpineUI.PlayClipInterrupt(config);
            }
            Debug.Log($"Play animation {config.BaseInfo.Name} for unit {unit.name}");
        }
    }
}
