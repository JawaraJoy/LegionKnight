using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Background", menuName = "Legion Knight/Background", order = 1)]
    public class BackgroundDefinition : ScriptableObject
    {
        [SerializeField]
        private OrnamentType m_StartOrnament = OrnamentType.NoOrnament;
        [SerializeField]
        private Sprite m_StartBackground;
        [SerializeField]
        private Sprite m_BaseLoop;
        [SerializeField]
        private Sprite m_EntranceOrnament;
        [SerializeField]
        private Sprite m_OrnamentLoop;

        public OrnamentType StartOrnament => m_StartOrnament;
        public Sprite StartBackground => m_StartBackground;
        public Sprite BaseLoop => m_BaseLoop;
        public Sprite EntranceOrnament => m_EntranceOrnament;
        public Sprite OrnamentLoop => m_OrnamentLoop;

        public void SetOrnament(int index)
        {
            OrnamentType ornament = (OrnamentType)index;
            GameManager.Instance.SetBackgroundOrnament(ornament);
        }
    }
}
