using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Rush
{
    public class SpineFlashController : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation m_SkeletonAnimation;

        private MeshRenderer m_Renderer;

        private MaterialPropertyBlock m_PropertyBlock;

        private static readonly int FlashColor =
            Shader.PropertyToID("_FlashColor");

        private static readonly int FlashAmount =
            Shader.PropertyToID("_FlashAmount");

        private Coroutine m_FlashCoroutine;

        private void Awake()
        {
            m_Renderer = m_SkeletonAnimation.GetComponent<MeshRenderer>();

            m_PropertyBlock = new MaterialPropertyBlock();

            SetFlash(Color.white, 0f);
        }

        public void FlashDamage()
        {
            if (m_FlashCoroutine != null)
                StopCoroutine(m_FlashCoroutine);

            m_FlashCoroutine = StartCoroutine(
                FlashRoutine(Color.white, 1f, 0.08f)
            );
        }

        private IEnumerator FlashRoutine(
            Color color,
            float intensity,
            float duration)
        {
            SetFlash(color, intensity);

            yield return new WaitForSeconds(duration);

            SetFlash(color, 0f);

            m_FlashCoroutine = null;
        }

        private void SetFlash(Color color, float amount)
        {
            m_Renderer.GetPropertyBlock(m_PropertyBlock);

            m_PropertyBlock.SetColor(FlashColor, color);
            m_PropertyBlock.SetFloat(FlashAmount, amount);

            m_Renderer.SetPropertyBlock(m_PropertyBlock);
        }
    }
}
