using UnityEngine;

namespace LegionKnight
{
    public class BossSpineAgent : MonoBehaviour
    {
        private BosEnemy GetBos()
        {
            BosEnemy bos = GameManager.Instance.SpawnedBosenemy;
            if (bos == null)
            {
                Debug.LogWarning("BosEnemy is not assigned.");
            }
            return bos;
        }
        public void SetAnim(SpineAnimDefinition anim)
        {
            GetBos().SetAnim(anim);
        }
        public void ChangeSpine(BosDefinition defi)
        {
            GetBos().ChangeSpine(defi);
        }
        public void PlayAnimationOnce(string key)
        {
            BosEnemy bos = GetBos();
            if (bos != null)
            {
                bos.PlayAnimationOnce(key);
            }
            else
            {
                Debug.LogWarning("BosEnemy is not assigned.");
            }
        }
    }
}
