
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Rush;

namespace LegionKnight
{
    public partial class CharacterSelectionView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CharacterSelectViewAsset;

        [SerializeField]
        private List<HeroSelectView> m_SpawnedCharacterSelectView = new();

        [SerializeField]
        private Transform m_SpawnContainer;

        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            m_SpawnedCharacterSelectView = new List<HeroSelectView>(m_SpawnContainer.GetComponentsInChildren<HeroSelectView>(true));
            List<HeroUnit> characterDecks = Player.Instance.HeroDeck.HeroUnits;
            foreach (HeroUnit unit in characterDecks)
            {
                if (GetSelectView(unit.HeroConfig) == null)
                {
                    SpawnCharacterSelectView(unit);
                }
                else
                {
                    GetSelectView(unit.HeroConfig).Init(unit);
                }
            }        
        }

        private HeroSelectView GetSelectView(HeroUnitConfig heroConfig)
        {
            HeroSelectView view = m_SpawnedCharacterSelectView.Find(x => x.HeroConfig == heroConfig);
            if (view == null)
            {
                return null;
            }
            return view;
        }
        private void SpawnCharacterSelectView(HeroUnit unit)
        {
            StartCoroutine(SpawningCharacterSelectView(unit));
        }
        private IEnumerator SpawningCharacterSelectView(HeroUnit unit)
        {
            AsyncOperationHandle<GameObject> handle = m_CharacterSelectViewAsset.InstantiateAsync(m_SpawnContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out HeroSelectView view))
                {
                    view.Init(unit);
                    m_SpawnedCharacterSelectView.Add(view);
                }
            }
        }

        private HeroSelectView[] GetCharacterSelectViews(RarityConfig rarityConfig)
        {
            return m_SpawnedCharacterSelectView.FindAll(x => x.HeroConfig.CollectibleField.RarityConfig == rarityConfig).ToArray();
        }

        public void ShowRarity(RarityConfig rarityConfig)
        {
            ShowRarityInternal(rarityConfig);
        }
        private void ShowRarityInternal(RarityConfig rarity)
        {
            foreach (HeroSelectView characterSelectView in m_SpawnedCharacterSelectView)
            {
                characterSelectView.Hide();
            }
            HeroSelectView[] view = GetCharacterSelectViews(rarity);
            foreach (HeroSelectView characterSelectView in view)
            {
                characterSelectView.Show();
            }
        }
        public void ShowAll()
        {
            foreach (HeroSelectView characterSelectView in m_SpawnedCharacterSelectView)
            {
                characterSelectView.Show();
            }
        }
        public void HideAll()
        {
            foreach (HeroSelectView characterSelectView in m_SpawnedCharacterSelectView)
            {
                characterSelectView.Hide();
            }
        }
        public void Init()
        {
            foreach (HeroSelectView view in m_SpawnedCharacterSelectView)
            {
                view.Init();
            }
        }
    }
}
