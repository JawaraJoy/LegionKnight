using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class ShieldView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_TextCount;
        [SerializeField]
        private UnityEvent m_OnCounted;
        public void SetCount(int count)
        {
            if (m_TextCount != null)
            {
                m_TextCount.text = count.ToString();
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI is not assigned in ShieldView.");
            }
            m_OnCounted?.Invoke();
            if (IsZero(count))
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private bool IsZero(int count)
        {
            return count <= 0;
        }
    }
    public partial class GameplayPanel
    {
        private ShieldView GetShieldView()
        {
            return GetBinding<ShieldView>();
        }

        public void SetShieldCount(int count)
        {
            var shieldView = GetShieldView();
            if (shieldView != null)
            {
                shieldView.SetCount(count);
            }
            else
            {
                Debug.LogWarning("ShieldView is not bound in GameplayPanel.");
            }
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetShieldCountView(int count)
        {
            GameplayPanel gm = GameManager.Instance.GetPanel<GameplayPanel>();
            if (gm != null)
            {
                gm.SetShieldCount(count);
            }
            else
            {
                Debug.LogWarning("GameplayPanel is not available in GameplayPanelAgent.");
            }
        }
    }
}
