using UnityEngine;

namespace Rush
{
    public class HomeBackgroundAgent : MonoBehaviour
    {
        public void Show()
        {
            HomeBackground.Instance.Show();
        }

        public void Hide()
        {
            HomeBackground.Instance.Hide();
        }
    }
}
