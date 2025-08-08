using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class Cooldown : MonoBehaviour
    {
        [SerializeField]
        private float m_CooldownTime;
        private float m_CurrentTime;
        [SerializeField]
        private TextMeshProUGUI m_CountdownText;
        [SerializeField]
        private Slider m_CountdownSlider;

        [SerializeField]
        private UnityEvent m_OnStart;
        [SerializeField]
        private UnityEvent m_OnDone;

        private AbilityDefinition m_AbilityDefinition;

        private Coroutine m_StartCooldownCoroutine;
        private IEnumerator CountingCooldown()
        {
            float time = 0f;
            m_OnStart.Invoke();
            while (time < m_CooldownTime)
            {
                time += Time.deltaTime;
                m_CurrentTime = time;
                m_CountdownText.text = Mathf.CeilToInt(m_CooldownTime - m_CurrentTime).ToString();
                float rateTime = m_CurrentTime/m_CooldownTime;
                m_CountdownSlider.value = rateTime;
                yield return null;
            }
            m_CountdownText.text = "0";
            m_OnDone.Invoke();
            yield return new WaitForEndOfFrame();
        }

        public void StartCooldown(AbilityDefinition ability)
        {
            m_AbilityDefinition = ability;
            CooldownStat cooldown = ability.CooldownStat;
            float cooldownTime = cooldown.CooldownTime;
            StartCooldownInternal(cooldownTime);
        }

        private void StartCooldownInternal(float cooldownTime)
        {
            SetCooldownTime(cooldownTime);
            m_StartCooldownCoroutine = StartCoroutine(CountingCooldown());
        }

        private void SetCooldownTime(float cooldown)
        {
            m_CooldownTime = cooldown;
        }
    }
}
