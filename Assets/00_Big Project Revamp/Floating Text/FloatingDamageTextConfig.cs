using TMPro;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "DamageText", menuName = "Rush/VFX/DamageText")]
    public class FloatingDamageTextConfig : ScriptableObject
    {
        [Header("Prefab")]
        [SerializeField] private FloatingDamageTextObject m_Prefab;

        [Header("Movement")]
        public float MoveSpeed = 2f;
        public float Lifetime = 1.2f;
        public float SprayRadius = 0.5f;

        [Header("Text")]
        public float FontSize = 4f;
        [SerializeField] private string m_BeforeText;
        [SerializeField] private string m_AfterText;

        [Header("Sprite Icon")]
        [SerializeField] private TMP_SpriteAsset m_SpriteAsset;
        [SerializeField] private SpriteAssetPosition m_SpritePosition = SpriteAssetPosition.After;

        [Header("Color")]
        public Gradient ColorOverLifetime;
        public FloatingDamageTextObject Prefab => m_Prefab;
        public TMP_SpriteAsset SpriteAsset => m_SpriteAsset;
        public string BeforeText => m_BeforeText;
        public string AfterText => m_AfterText;
        public SpriteAssetPosition SpritePosition => m_SpritePosition;
    }

    public enum SpriteAssetPosition
    {
        Before,
        After
    }
}