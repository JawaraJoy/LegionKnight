
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using LegionKnight;
using MoreMountains.Tools;

namespace Rush
{
    public partial class HeroSelectTabView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_HeroSelectViewAsset;
        [SerializeField]
        private HeroView m_HeroView;

        [SerializeField]
        private HeroSelectView[] m_StartingHeroSelectView;
        [SerializeField]
        private List<HeroSelectView> m_SpawnedHeroSelectView = new();

        [SerializeField]
        private Transform m_SpawnContainer;
        [SerializeField, MMReadOnly]
        private List<HeroUnit> characterDecks = new();
        public void Init()
        {
            InitInternal();
        }

        private void InitInternal()
        {
            characterDecks = Player.Instance.HeroesCollection.HeroUnits;

            foreach (HeroSelectView view in m_StartingHeroSelectView)
            {
                view.Init(m_HeroView);
                m_SpawnedHeroSelectView.Add(view);
            }
            foreach (HeroUnit unit in characterDecks)
            {
                if (GetSelectView(unit.HeroConfig) == null)
                {
                    SpawnHeroSelectView(unit);
                }
                else
                {
                    GetSelectView(unit.HeroConfig).Init(unit, m_HeroView);
                }
            }
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            ShowAllSelectInternal();
        }

        private HeroSelectView GetSelectView(HeroUnitConfig heroConfig)
        {
            HeroSelectView view = m_SpawnedHeroSelectView.Find(x => x.HeroConfig == heroConfig);
            if (view == null)
            {
                return null;
            }
            return view;
        }
        private bool HasSelectView(HeroUnitConfig heroConfig, out HeroSelectView heroSelectView)
        {
            heroSelectView = GetSelectView(heroConfig);
            return heroSelectView != null;

        }
        private void SpawnHeroSelectView(HeroUnit unit)
        {
            RushGameManager.Instance.StartCoroutine(SpawningHeroSelectView(unit));
        }
        private IEnumerator SpawningHeroSelectView(HeroUnit unit)
        {
            AsyncOperationHandle<GameObject> handle = m_HeroSelectViewAsset.InstantiateAsync(m_SpawnContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out HeroSelectView view))
                {
                    view.Init(unit, m_HeroView);
                    m_SpawnedHeroSelectView.Add(view);
                }
            }
        }

        private HeroSelectView[] GetHeroSelectViews(RarityConfig rarityConfig)
        {
            return m_SpawnedHeroSelectView.FindAll(x => x.HeroConfig.CollectibleField.RarityConfig == rarityConfig).ToArray();
        }

        public void ShowRarity(RarityConfig rarityConfig)
        {
            ShowRarityInternal(rarityConfig);
        }
        private void ShowRarityInternal(RarityConfig rarity)
        {
            foreach (HeroSelectView heroSelectView in m_SpawnedHeroSelectView)
            {
                heroSelectView.Hide();
            }
            HeroSelectView[] view = GetHeroSelectViews(rarity);
            foreach (HeroSelectView heroSelectView in view)
            {
                heroSelectView.Show();
            }
        }
        private void ShowAllSelectInternal()
        {
            foreach (HeroSelectView heroSelectView in m_SpawnedHeroSelectView)
            {
                heroSelectView.Show();
            }
        }
        public void ShowAll()
        {
            ShowAllSelectInternal();
        }
        public void HideAll()
        {
            foreach (HeroSelectView heroSelectView in m_SpawnedHeroSelectView)
            {
                heroSelectView.Hide();
            }
        }
    }
}
