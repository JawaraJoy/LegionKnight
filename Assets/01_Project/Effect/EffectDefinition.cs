using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Legion Knight/Effect", order = 1)]
    public class EffectDefinition : ScriptableObject
    {
        public void PlayAnimation(string animationName)
        {
            GameManager.Instance.PlayEffect(this, animationName);

        }
        public void PauseAnimation()
        {

            GameManager.Instance.PauseEffect(this);
        }
        public void ResumeAnimation()
        {

            GameManager.Instance.ResumeEffect(this);
        }
        public void SpeedUpAnimation()
        {

            GameManager.Instance.SpeedUpEffect(this);
        }
        public void ResetSpeed()
        {

            GameManager.Instance.ResetSpeed(this);
        }
    }
}
