using UnityEngine;

namespace LegionKnight
{
    public abstract class GuardActivation : MonoBehaviour
    {
        [SerializeField]
        protected Sprite m_GuardSprite;
        [SerializeField]
        protected float m_GuardDuration = 5f;
        private void Start()
        {
            GuardActive();
        }

        protected abstract void GuardActive();
    }
}
