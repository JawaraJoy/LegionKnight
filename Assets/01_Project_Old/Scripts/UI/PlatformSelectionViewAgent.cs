using UnityEngine;

namespace LegionKnight
{
    public class PlatformSelectionViewAgent : MonoBehaviour
    {
        private HeroPanel GetCharacterPanel()
        {
            return CanvasManager.Instance.GetPanel<HeroPanel>();
        }

        private PlatformSelectionView GetPlatformSelectionView()
        {
            return GetCharacterPanel().GetBinding<PlatformSelectionView>();
        }

        public void SpawnPlatformSelectionView(PlatformUnit unit)
        {
            GetPlatformSelectionView().SpawnPlatformSelect(unit);
        }
    }
}
