using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class NewUnlockedPopUpPanel : PanelView
    {

        [SerializeField]
        private PopUpView[] m_PopUps;

        private int m_CurrentIndex = 0;

        public void ShowPopUp(ScriptableObject popUpData)
        {
            Time.timeScale = 1f;
            if (m_PopUps == null || m_PopUps.Length == 0)
                return;

            // Ambil popup berdasarkan index
            PopUpView popUp = m_PopUps[m_CurrentIndex];

            // Tampilkan data baru (override jika perlu)
            popUp.ShowPopUp(popUpData);

            // Pindahkan ke paling bawah (last child)
            popUp.transform.SetAsLastSibling();

            // Geser index (circular)
            m_CurrentIndex++;

            if (m_CurrentIndex >= m_PopUps.Length)
                m_CurrentIndex = 0;
        }
    }

}
