using UnityEngine;

namespace LegionKnight
{
    public class CardSelectionViewAgent : MonoBehaviour
    {
        private HeroPanel GetCharacterPanel()
        {
            return CanvasManager.Instance.GetPanel<HeroPanel>();
        }

        private CardSelectionView GetCardSelectionView()
        {
            return GetCharacterPanel().GetBinding<CardSelectionView>();
        }

        public void SpawnCardSelectionView(CardUnit unit)
        {
            GetCardSelectionView().SpawnCardSelect(unit);
        }
    }
}
