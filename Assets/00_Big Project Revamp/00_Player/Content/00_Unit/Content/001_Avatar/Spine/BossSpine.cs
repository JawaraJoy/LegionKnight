using UnityEngine;

namespace LegionKnight
{
    public class BossSpine : CharacterSpine
    {
        
    }

    public partial class BosEnemy
    {
        [SerializeField]
        private BossSpine m_BossSpine;

        public BossSpine BossSpine => m_BossSpine;

        public void SetAnim(SpineAnimDefinition anim)
        {
            if (m_BossSpine != null)
            {
                m_BossSpine.SetAnim(anim);
            }
            else
            {
                Debug.LogWarning("BossSpine is not assigned.");
            }
        }
        public void ChangeSpine(BosDefinition defi)
        {
            m_BossSpine.ChangeSpine(defi);
        }
        public void PlayAnimationOnce(string key)
        {
            if (m_BossSpine != null)
            {
                m_BossSpine.PlayAnimationOnce(key);
            }
            else
            {
                Debug.LogWarning("BossSpine is not assigned.");
            }
        }
    }
}
