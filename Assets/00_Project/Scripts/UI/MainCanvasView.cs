using UnityEngine;

namespace LegionKnight
{
    public partial class MainCanvasView : CanvasView
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private MainCanvasView m_MainCanvas;

        protected T GetPanelInternal<T>() where T : PanelView
        {
            return m_MainCanvas.GetPanel<T>();
        }
        protected PanelView GetPanelInternal(string uniqueId)
        {
            return m_MainCanvas.GetPanel(uniqueId);
        }
        public bool IsShowPanel(string uniqueId)
        {
            return GetPanelInternal(uniqueId).IsShow;
        }
        public void ShowMainCanvas()
        {
            m_MainCanvas.Show();
        }
        public void HideMainCanvas()
        {
            m_MainCanvas.Hide();
        }

        protected void ShowPanelInternal(string uniqueId)
        {
            m_MainCanvas.ShowPanel(uniqueId);
        }
        protected void HidePanelInternal(string uniqueId)
        {
            m_MainCanvas.HidePanel(uniqueId);
        }
        public virtual void ShowPanel(string uniqueId)
        {
            ShowPanelInternal(uniqueId);
        }
        public virtual void HidePanel(string uniqueId)
        {
            HidePanelInternal(uniqueId);
        }
    }
}
