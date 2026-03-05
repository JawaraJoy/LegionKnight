using TMPro;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "FloatingDamageTextConfig", menuName = "Rush/VFX/FloatingDamageTextConfig")]
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
        public string BeforeText;
        public string AfterText;

        [Header("Color")]
        public Gradient ColorOverLifetime;

        [Header("Scale")]
        public AnimationCurve ScaleOverLifetime = AnimationCurve.Linear(0, 1, 1, 1);

        [Header("Icon")]
        public TMP_SpriteAsset SpriteAsset;
        public SpriteAssetPosition SpriteAssetPosition = SpriteAssetPosition.After;
        public FloatingDamageTextObject Prefab => m_Prefab;
    }
    public enum SpriteAssetPosition
    {
        Before,
        After
    }
}
