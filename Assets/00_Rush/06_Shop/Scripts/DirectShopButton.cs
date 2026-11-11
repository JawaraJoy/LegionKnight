using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class DirectShopButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;
        private void Start()
        {
            m_Button.onClick.AddListener(ShowShopPanel);
        }
        private void ShowShopPanel()
        {
            CanvasManager.Instance.GetPanel<ShopPanel>().Show();
        }
    }
}
