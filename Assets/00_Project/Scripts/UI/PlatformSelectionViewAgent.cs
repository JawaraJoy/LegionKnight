using UnityEngine;

namespace LegionKnight
{
    public class PlatformSelectionViewAgent : MonoBehaviour
    {
        private CharacterPanel GetCharacterPanel()
        {
            return CanvasManager.Instance.GetPanel<CharacterPanel>();
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
