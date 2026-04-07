using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

namespace Rush
{
    public class PlayerReborn : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private Unit m_PlayerHerounit;
        private void Start()
        {
            m_PlayerHerounit = RushPlayer.Instance.Unit;
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
        }
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerReborn m_Reborn;
        public PlayerReborn Reborn => m_Reborn;
    }
}
