using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;

namespace LegionKnight
{
    [System.Serializable]
    public partial struct ForceEffect
    {
        public string ForceName;
        public Vector2 ForceDirection;
    }
    public partial class DeathEffect : MonoBehaviour
    {
        // force the gameobject to move
        [SerializeField]
        private List<ForceEffect> m_ForceEffects = new();
        
        [SerializeField]
        private Rigidbody2D m_Rb;

        [SerializeField]
        private bool m_UsePause;
        [SerializeField]
        private float m_PauseDelay;
        [SerializeField]
        private UnityEvent m_OnDeath = new();
        private void OnDeathInvoke()
        {
            m_OnDeath?.Invoke();
            Player.Instance.Death();
        }
        private ForceEffect GetForceEffect(string forceName)
        {
            ForceEffect match = m_ForceEffects.Find(x => x.ForceName == forceName);
            return match;
        }
        public void ApplyForce(string forceName)
        {
            Vector2 force = GetForceEffect(forceName).ForceDirection;
            m_Rb.AddForce(force, ForceMode2D.Impulse);
            ApplyPause();
            OnDeathInvoke();
        }
        private void ApplyPause()
        {
            if (!m_UsePause) return;
            StartCoroutine(ApplyPausing());
        }
        private IEnumerator ApplyPausing()
        {
            yield return new WaitForSeconds(m_PauseDelay);
            GameManager.Instance.ShowPanel(PanelId.GameOverPanelId);
            m_Rb.AddForce(Vector2.zero, ForceMode2D.Impulse);
        }
    }
    public partial class Character
    {
        [SerializeField]
        private CharacterJump m_Jump;
        [SerializeField]
        private Collider2D m_Collider;
        [SerializeField]
        private TouchDown m_TouchDown;
        public void Death()
        {
            m_Jump.SetCanJump(false);
            m_Collider.enabled = false;
            m_TouchDown.SetCanContact(false);
            
        }
        public void Reborn()
        {
            m_Jump.SetCanJump(true);
            m_Collider.enabled = true;
            m_TouchDown.SetCanContact(true);
        }
        public void SetPosition(Vector2 post)
        {
            transform.position = post;
        }
    }

    public partial class Player
    {
        public void Death()
        {
            m_Character.Death();
        }
        public void Reborn()
        {
            m_Character.Reborn();
        }
        public void SetPosition(Vector2 post)
        {
            m_Character.SetPosition(post);
        }
    }
}
