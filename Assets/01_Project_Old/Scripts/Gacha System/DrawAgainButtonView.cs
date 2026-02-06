using UnityEngine;

namespace LegionKnight
{
    public partial class DrawAgainButtonView : DrawButtonView
    {
        /*private GachaHandler m_Handler;

        private GachaHandler Handler
        {
            get
            {
                if (m_Handler == null)
                    m_Handler = GameManager.Instance.GachaMananger;
                return m_Handler;
            }
        }
        public void Refresh()
        {
            if (Handler == null)
                return;

            var cost = Handler.LastDrawCost;
            if (cost == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            SetButtonView(cost);
        }
        private void Start()
        {
            //m_DrawButton.onClick.RemoveAllListeners();
            m_DrawButton.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            if (Handler == null)
                return;

            switch (Handler.LastDrawType)
            {
                case LastDrawType.Single:
                    m_Handler.PerformSingleDraw();
                    break;

                case LastDrawType.Multi:
                    m_Handler.PerformMultiDraw();
                    break;
            }
        }*/
    }
}
