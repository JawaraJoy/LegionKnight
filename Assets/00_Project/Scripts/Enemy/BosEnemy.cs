using UnityEngine;

namespace LegionKnight
{
    public partial class BosEnemy : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_BosForm;
        private void Start()
        {
            
        }
        public void Init(BosDefinition definition)
        {
            m_BosDefinition = definition;
            m_Damageable.Init(0, m_BosDefinition.Health);
            m_BosForm.sprite = m_BosDefinition.Icon;
        }
        public void SetLocalPosition(Vector2 post)
        {
            transform.localPosition = post;
        }
    }
}
