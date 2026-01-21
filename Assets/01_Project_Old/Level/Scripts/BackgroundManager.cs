using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace LegionKnight
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private Background m_Background;

        public void SetOrnament(OrnamentType ornament)
        {
            if (m_Background == null)
            {
                Debug.LogError("Background is not set. Please initialize the BackgroundManager with a Background instance.");
                return;
            }
            m_Background.SetOrnament(ornament);
        }

        public void Initialize(LevelDefinition level)
        {
            if (m_Background == null)
            {
                Debug.LogError("Background is not set. Please assign a Background instance to the BackgroundManager.");
                return;
            }
            m_Background.Initialize(level);
        }
        public void SetBackGround(Background background)
        {
            m_Background = background;
        }
    }

    public partial class GameManager
    {
        [SerializeField]
        private BackgroundManager m_BackgroundManager;
        public void InitializeBackground(LevelDefinition level)
        {
            m_BackgroundManager.Initialize(level);
        }
        public void SetBackgroundOrnament(OrnamentType ornament)
        {
            m_BackgroundManager.SetOrnament(ornament);
        }
        public void SetBackGround(Background background)
        {
            m_BackgroundManager.SetBackGround(background);
        }
    }
}
