using UnityEngine;

namespace LegionKnight
{
    public class CardSelectionViewAgent : MonoBehaviour
    {
        private HeroPanel GetCharacterPanel()
        {
            return CanvasManager.Instance.GetPanel<HeroPanel>();
        }

        private CardSelectTabView GetCardSelectionView()
        {
            return GetCharacterPanel().GetBinding<CardSelectTabView>();
        }

        public void SpawnCardSelectionView(CardUnit unit)
        {
            GetCardSelectionView().SpawnCardSelect(unit);
        }
    }
}
