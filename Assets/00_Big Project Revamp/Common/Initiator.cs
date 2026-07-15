using LegionKnight;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Initiator : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnStart;

        IEnumerator Start()
        {
            CanvasManager.Instance.GetPanel<NewGameplayPanel>().Hide();
            Application.targetFrameRate = 60;
            yield return new WaitForSeconds(1f);
            m_OnStart?.Invoke();
            CanvasManager.Instance.GetPanel<NewGameplayPanel>().Show();
        }
    }
}
