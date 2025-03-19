using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class CharacterSelectionView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CharacterSelectViewAsset;

        private List<CharacterSelectView> m_SpawnedCharacterSelectView = new();
    }
}
