using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Rush
{
    public class FloatingDamageTextObject : MonoBehaviour, IUpdater
    {
        [SerializeField] private TextMeshPro m_Text;

        private FloatingDamageTextSpawner m_Spawner;
        private FloatingDamageTextConfig m_Config;

        private float m_Timer;

        public bool IsActive => gameObject.activeInHierarchy;

        [SerializeField, MMReadOnly]
        private PhysicsMode m_PhysicMode = PhysicsMode.Physics2D;

        private void Start()
        {
            m_PhysicMode = RushGameManager.Instance.GameConfig.PhysicsMode;
        }

        public void Setup(int amount, FloatingDamageTextSpawner spawner, Vector3 pos)
        {
            m_Spawner = spawner;
            m_Config = spawner.Config;

            transform.position = pos;

            m_Text.spriteAsset = m_Config.SpriteAsset;
            m_Text.fontSize = m_Config.FontSize;

            m_Text.text = BuildText(amount);

            m_Timer = 0f;

            gameObject.SetActive(true);
        }

        private string BuildText(int amount)
        {
            string sprite = m_Config.SpriteAsset != null ? "<sprite=0>" : "";

            if (m_Config.SpritePosition == SpriteAssetPosition.Before)
                return $"{m_Config.BeforeText}{sprite}{amount}{m_Config.AfterText}";

            return $"{m_Config.BeforeText}{amount}{sprite}{m_Config.AfterText}";
        }

        public void Tick()
        {
            if (m_Config == null) return;

            m_Timer += Time.deltaTime;

            float t = m_Timer / m_Config.Lifetime;

            Move();

            // Gradient color
            m_Text.color = m_Config.ColorOverLifetime.Evaluate(t);

            if (m_Timer >= m_Config.Lifetime)
                gameObject.SetActive(false);
        }

        private void Move()
        {
            float speed = m_Config.MoveSpeed * Time.deltaTime;

            if (m_PhysicMode == PhysicsMode.Physics2D)
                transform.position += new Vector3(0f, speed, 0f);
            else
                transform.position += Vector3.up * speed;
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
    }
}