using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Rush
{
    public class FloatingDamageTextObject : MonoBehaviour, IUpdater
    {
        [SerializeField] private TextMeshPro m_Text;

        private FloatingDamageTextConfig m_Config;

        private float m_Timer;
        private Vector3 m_MoveDirection;
        private Vector3 m_BaseScale;

        public bool IsActive => gameObject.activeInHierarchy;

        [SerializeField, MMReadOnly]
        private PhysicsMode m_PhysicMode = PhysicsMode.Physics2D;

        private void Start()
        {
            m_PhysicMode = RushGameManager.Instance.GameConfig.PhysicsMode;
        }

        public void Setup(int amount, FloatingDamageTextSpawner spawner, Vector3 pos)
        {
            m_Config = spawner.Config;

            transform.position = pos;

            m_Text.spriteAsset = m_Config.SpriteAsset;
            m_Text.fontSize = m_Config.FontSize;

            m_Text.text = BuildText(amount);

            m_Timer = 0f;

            m_BaseScale = Vector3.one;
            transform.localScale = m_BaseScale;

            m_MoveDirection = GetSprayDirection();

            gameObject.SetActive(true);
        }

        private string BuildText(int amount)
        {
            if (amount <= 0)
                return $"{m_Config.BeforeText}{m_Config.AfterText}";
            if (m_Config.SpriteAsset == null)
                return $"{m_Config.BeforeText}{amount}{m_Config.AfterText}";
            string sprite = "<sprite=0 tint=1>";

            if (m_Config.SpriteAssetPosition == SpriteAssetPosition.Before)
                return $"{sprite}{m_Config.BeforeText}{amount}{m_Config.AfterText}";
            
            return $"{m_Config.BeforeText}{amount}{m_Config.AfterText}{sprite}";
        }

        public void Tick()
        {
            if (m_Config == null) return;

            m_Timer += Time.deltaTime;

            float t = m_Timer / m_Config.Lifetime;

            Move();

            m_Text.color = m_Config.ColorOverLifetime.Evaluate(t);

            float scale = m_Config.ScaleOverLifetime.Evaluate(t);
            transform.localScale = m_BaseScale * scale;

            if (m_Timer >= m_Config.Lifetime)
                gameObject.SetActive(false);
        }

        private void Move()
        {
            float speed = m_Config.MoveSpeed * Time.deltaTime;
            transform.position += m_MoveDirection * speed;
        }

        private Vector3 GetSprayDirection()
        {
            Vector2 random = Random.insideUnitCircle * m_Config.SprayRadius;

            if (m_PhysicMode == PhysicsMode.Physics2D)
                return new Vector3(random.x, Mathf.Abs(random.y) + 1f, 0f).normalized;

            return new Vector3(random.x, Mathf.Abs(random.y) + 1f, random.y).normalized;
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            //UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
    }
}