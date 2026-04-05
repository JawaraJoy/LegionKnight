using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    public partial class HeroesCollection : MonoBehaviour
    {
        [SerializeField]
        private HeroUnitConfig m_DefaultHeroConfig;
        [SerializeField]
        private HeroUnitConfig m_UsedHeroConfig;
        [SerializeField]
        private HeroUnitConfig m_SelectedHeroConfig;
        [SerializeField]
        private List<HeroUnit> m_HeroUnits = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnCharacterUsed = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnSelectedCharacter = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnCharacterLevelUp = new();
        [SerializeField]
        private UnityEvent<int> m_OnCharacterLevelUpAmount = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnCharacterOwned = new();
        [SerializeField]
        private UnityEvent<int> m_OnCharacterOwnedAmount = new();

        public UnityEvent<HeroUnitConfig> OnCharacterLevelUp => m_OnCharacterLevelUp;
        public UnityEvent<HeroUnitConfig> OnCharacterStarUp => m_OnCharacterStarUp;
        public UnityEvent<HeroUnitConfig> OnCharacterOwned => m_OnCharacterOwned;
        public UnityEvent<int> OnCharacterOwnedAmount => m_OnCharacterOwnedAmount;
        public UnityEvent<int> OnCharacterLevelUpAmount => m_OnCharacterLevelUpAmount;
        public UnityEvent<HeroUnitConfig> OnInitialized => m_OnInitialized;
        public UnityEvent<HeroUnitConfig> OnCharacterUsed => m_OnCharacterUsed;
        public UnityEvent<HeroUnitConfig> OnSelectedCharacter => m_OnSelectedCharacter;

        public HeroUnitConfig DefaultHero => m_DefaultHeroConfig;
        public List<HeroUnit> HeroUnits => m_HeroUnits;
        public HeroUnitConfig UsedHero => m_UsedHeroConfig;
        public HeroUnitConfig SelectedHero => m_SelectedHeroConfig;
        private HeroUnit GetHeroUnitInternal(HeroUnitConfig config)
        {
            HeroUnit match = m_HeroUnits.Find(x => x.HeroConfig == config);
            return match;
        }
        private HeroUnit GetHeroUnitInternal(string id)
        {
            HeroUnit match = m_HeroUnits.Find(x => x.HeroConfig.BaseInfo.Id == id);
            return match;
        }
        public HeroUnit GetHeroUnit(HeroUnitConfig config)
        {
            return GetHeroUnitInternal(config);
        }

        public void Init()
        {
            if (UnityService.Instance.HasData("usedcharacter"))
            {
                m_UsedHeroConfig = GetHeroUnitInternal(UnityService.Instance.GetData<string>("usedcharacter")).HeroConfig;
                m_SelectedHeroConfig = m_UsedHeroConfig;
            }
            else
            {
                m_UsedHeroConfig = m_DefaultHeroConfig;
                UnityService.Instance.SaveData("usedcharacter", m_UsedHeroConfig.BaseInfo.Id);
            }
            OnInitializedInvoke();
        }
        private void OnInitializedInvoke()
        {
            m_OnInitialized?.Invoke(m_UsedHeroConfig);
            foreach(HeroUnit unit in m_HeroUnits)
            {
                unit.Init();
                //unit.Definition.InitAsOwner();
            }
            RushPlayer.Instance.Init(m_UsedHeroConfig);
            Debug.Log("Heroes Collection Initialized");
            CanvasManager.Instance.GetPanel<PreparationPanel>().HeroTabView.Init();
        }
        public void SetOwned(HeroUnitConfig config, bool set)
        {
            GetHeroUnitInternal(config).SetOwned(set);
            if (set)
            {
                OnHeroOwnedInvoke(config);
            }
        }

        private void OnHeroOwnedInvoke(HeroUnitConfig config)
        {
            m_OnCharacterOwned?.Invoke(config);
            int ownedAmount = 0;
            foreach (HeroUnit unit in m_HeroUnits)
            {
                if (unit.Owned)
                {
                    ownedAmount++;
                }
            }
            m_OnCharacterOwnedAmount?.Invoke(ownedAmount);
        }

        public void SetUsedHero()
        {
            m_UsedHeroConfig = m_SelectedHeroConfig;
            GetHeroUnitInternal(m_UsedHeroConfig).SetIsUsed(true);
            foreach (HeroUnit unit in m_HeroUnits)
            {
                if (unit.HeroConfig != m_UsedHeroConfig)
                {
                    unit.SetIsUsed(false);
                }
            }
            UnityService.Instance.SaveData("usedcharacter", m_UsedHeroConfig.BaseInfo.Id);
            OnCharacterUsedInvoke();
        }
        public void SetSelectedHero(HeroUnitConfig config)
        {
            m_SelectedHeroConfig = config;
            OnSelectedCharacterInvoke();
        }
        private void OnCharacterUsedInvoke()
        {
            m_OnCharacterUsed?.Invoke(m_UsedHeroConfig);
            RushPlayer.Instance.Init(m_UsedHeroConfig);
        }
        private void OnSelectedCharacterInvoke()
        {
            m_OnSelectedCharacter?.Invoke(m_SelectedHeroConfig);
            //CanvasManager.Instance.SetHeroSelected(m_SelectedHeroConfig);
        }
        
    }
}
