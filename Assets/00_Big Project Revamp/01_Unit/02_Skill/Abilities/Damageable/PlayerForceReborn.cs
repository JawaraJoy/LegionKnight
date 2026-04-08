using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace Rush
{
    public class PlayerForceReborn : MonoBehaviour, IReseter
    {
        [SerializeField, MMReadOnly]
        private Unit m_PlayerHerounit;

        private bool m_CanForceReborn;
        public bool CanForceReborn => m_CanForceReborn;
        private void Start()
        {
            m_PlayerHerounit = RushPlayer.Instance.Unit;
            SetCanForceRebornInternal(true);
        }
        private void SetCanForceRebornInternal(bool set)
        {
            m_CanForceReborn = set;
        }
        public void SetCanForceReborn(bool set)
        {
            SetCanForceRebornInternal(set);
        }
        public void ForcingReborn(float delay)
        {
            StartCoroutine(ForcingRebornInternal(delay));
        }
        private IEnumerator ForcingRebornInternal(float delay)
        {
            yield return new WaitForSeconds(delay);
            Vector2 lastpost = RushGameManager.Instance.StageManager.PlatformHandler.LastContactPoint;
            Vector2 offsite = new (lastpost.x, lastpost.y + 3);
            

            if (m_PlayerHerounit.HasBind(out Damageable damageable))
            {
                damageable.Reborn(m_PlayerHerounit.Config.RebornConfig);
                RushPlayer.Instance.SetPosition(offsite);
            }
            yield return new WaitForSeconds(1f);
            RushGameManager.Instance.StageManager.Resume();
            SetCanForceRebornInternal(false);
        }

        public void ResetProgression()
        {
            SetCanForceRebornInternal(true);
        }
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerForceReborn m_Reborn;
        public PlayerForceReborn Reborn => m_Reborn;
    }
}
