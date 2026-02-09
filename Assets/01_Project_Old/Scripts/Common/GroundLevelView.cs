using UnityEngine;

namespace LegionKnight
{
    public partial class GroundLevelView : View
    {
        [SerializeField]
        private SpriteRenderer m_BaseGround;
        [SerializeField]
        private SpriteRenderer m_LeftCliff;
        [SerializeField]
        private SpriteRenderer m_RightCliff;
        [SerializeField]
        private SpriteRenderer m_Tree;
        [SerializeField]
        private SpriteRenderer m_Mountain;
        [SerializeField]
        private SpriteRenderer m_Background;
        public void Init(LevelOrnament set)
        {
            SetBaseGround(set.BaseGround);
            SetLeftCliff(set.LeftCliff);
            SetRightCliff(set.RightCliff);
            SetTree(set.Tree);
            SetMountain(set.Mountain);
            SetBackground(set.Background);
        }

        private void SetBaseGround(Sprite sprite)
        {
            m_BaseGround.sprite = sprite;
        }
        private void SetLeftCliff(Sprite sprite)
        {
            m_LeftCliff.sprite = sprite;
        }
        private void SetRightCliff(Sprite sprite)
        {
            m_RightCliff.sprite = sprite;
        }
        private void SetTree(Sprite sprite)
        {
            m_Tree.sprite = sprite;
        }
        private void SetMountain(Sprite sprite)
        {
            m_Mountain.sprite = sprite;
        }
        private void SetBackground(Sprite sprite)
        {
            m_Background.sprite = sprite;
        }
    }

    public partial class LevelObject
    {
        [SerializeField]
        private GroundLevelView m_GroundLevelView;

        public void SetGroundLevelView(LevelOrnament set)
        {
            m_GroundLevelView.Init(set);
        }
    }
}
