using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class CharacterSelectionView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CharacterSelectViewAsset;

        [SerializeField, ReadOnly]
        private List<CharacterSelectView> m_SpawnedCharacterSelectView = new();

        [SerializeField]
        private Transform m_SpawnContainer;

        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            m_SpawnedCharacterSelectView = new List<CharacterSelectView>(m_SpawnContainer.GetComponentsInChildren<CharacterSelectView>(true));
            List<CharacterUnit> characterDecks = Player.Instance.CharacterUnits;
            foreach (CharacterUnit unit in characterDecks)
            {
                if (GetSelectView(unit.Definition) == null)
                {
                    SpawnCharacterSelectView(unit);
                }
            }
        }

        private CharacterSelectView GetSelectView(CharacterDefinition defi)
        {
            CharacterSelectView view = m_SpawnedCharacterSelectView.Find(x => x.Definition == defi);
            if (view == null)
            {
                return null;
            }
            return view;
        }
        private void SpawnCharacterSelectView(CharacterUnit unit)
        {
            StartCoroutine(SpawningCharacterSelectView(unit));
        }
        private IEnumerator SpawningCharacterSelectView(CharacterUnit unit)
        {
            AsyncOperationHandle<GameObject> handle = m_CharacterSelectViewAsset.InstantiateAsync(m_SpawnContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out CharacterSelectView view))
                {
                    view.Init(unit);
                    m_SpawnedCharacterSelectView.Add(view);
                }
            }
        }

        private CharacterSelectView[] GetCharacterSelectViews(Rarity rarity)
        {
            return m_SpawnedCharacterSelectView.FindAll(x => x.Definition.Rarity == rarity).ToArray();
        }

        public void ShowRarity(int rarityIndex)
        {
            Rarity rarity = (Rarity)rarityIndex;
            ShowRarity(rarity);
        }
        private void ShowRarity(Rarity rarity)
        {
            foreach (CharacterSelectView characterSelectView in m_SpawnedCharacterSelectView)
            {
                characterSelectView.Hide();
            }
            CharacterSelectView[] view = GetCharacterSelectViews(rarity);
            foreach (CharacterSelectView characterSelectView in view)
            {
                characterSelectView.Show();
            }
        }
        public void ShowAll()
        {
            foreach (CharacterSelectView characterSelectView in m_SpawnedCharacterSelectView)
            {
                characterSelectView.Show();
            }
        }

        public void Init()
        {
            foreach (CharacterSelectView view in m_SpawnedCharacterSelectView)
            {
                view.Init();
            }
        }
    }
}
