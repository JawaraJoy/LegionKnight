using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class PlatformUnit
    {
        [SerializeField]
        private bool m_IsOwned;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private StandbyPlatformDefinition m_StanbyPlatform;
        public bool IsOwned => m_IsOwned = m_Amount > 0;
        public int Amount => m_Amount;
        public StandbyPlatformDefinition StanbyPlatform => m_StanbyPlatform;
        public PlatformUnit(StandbyPlatformDefinition stanbyPlatform)
        {
            m_StanbyPlatform = stanbyPlatform;
            m_IsOwned = false;
        }
        public void AddAmount(int add)
        {
            m_Amount += add;
            UnityService.Instance.SaveData(m_StanbyPlatform.Id + "amount", m_Amount);
            m_IsOwned = m_Amount > 0;
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_StanbyPlatform.Id + "amount"))
            {
                m_Amount = UnityService.Instance.GetData<int>(m_StanbyPlatform.Id + "amount");
                
            }
            else
            {
                m_Amount = 0;
                UnityService.Instance.SaveData(m_StanbyPlatform.Id + "amount", m_Amount);
            }
            m_IsOwned = m_Amount > 0;
        }
    }
    public partial class PlatformDeck : MonoBehaviour
    {
        [SerializeField]
        private StandbyPlatformDefinition m_UsedStanbyPlatform;
        [SerializeField]
        private StandbyPlatformDefinition m_SelectedStandbyPlatform;
        [SerializeField]
        private List<PlatformUnit> m_Deck = new();

        [SerializeField]
        private UnityEvent<StandbyPlatformDefinition> m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<StandbyPlatformDefinition> m_OnPlatformUsed = new();
        [SerializeField]
        private UnityEvent<StandbyPlatformDefinition> m_OnSelectedPlatform = new();

        private PlatformUnit GetPlatformOwnedInternal(StandbyPlatformDefinition platform)
        {
            foreach (var platformOwned in m_Deck)
            {
                if (platformOwned.StanbyPlatform == platform)
                {
                    return platformOwned;
                }
            }
            return null;
        }
        public PlatformUnit GetPlatformOwned(StandbyPlatformDefinition platform)
        {
            return GetPlatformOwnedInternal(platform);
        }
        public bool IsPlatformOwned(StandbyPlatformDefinition platform)
        {
            var platformOwned = GetPlatformOwnedInternal(platform);
            return platformOwned != null && platformOwned.IsOwned;
        }
        public StandbyPlatformDefinition GetUsedStanbyPlatform()
        {
            return m_UsedStanbyPlatform;
        }
        public void AddPlatformAmount(StandbyPlatformDefinition platform, int add)
        {
            AddPlatformAmountInternal(platform, add);
        }
        public void AddPlatformAmountInternal(StandbyPlatformDefinition platform, int add)
        {
            var platformOwned = GetPlatformOwnedInternal(platform);
            platformOwned?.AddAmount(add);
        }
        public void SetUsedStandbyPlatform()
        {
            m_UsedStanbyPlatform = m_SelectedStandbyPlatform;
            OnPlatformUsedInvoke();
        }
        public void SelectStandbyPlatform(StandbyPlatformDefinition platform)
        {
            m_SelectedStandbyPlatform = platform;
            OnSelectedPlatformInvoke();
        }
        public void AddPlayerStandbyPlatform()
        {
            if (GetPlatformOwnedInternal(m_UsedStanbyPlatform).IsOwned)
            {
                GameManager.Instance.AddStandbyPlatform(new List<StandbyPlatformDefinition> { m_UsedStanbyPlatform });
                AddPlatformAmountInternal(m_UsedStanbyPlatform, -1);
            }
        }
        public void Init()
        {
            OnInitializedInvoke();
        }
        private void OnInitializedInvoke()
        {
            m_OnInitialized?.Invoke(m_UsedStanbyPlatform);
            foreach (PlatformUnit unit in m_Deck)
            {
                unit.Init();
            }
        }
        private void OnSelectedPlatformInvoke()
        {
            m_OnSelectedPlatform?.Invoke(m_SelectedStandbyPlatform);
            GameManager.Instance.SetPlatformSelected(m_SelectedStandbyPlatform);
        }
        private void OnPlatformUsedInvoke()
        {
            m_OnPlatformUsed?.Invoke(m_UsedStanbyPlatform);
        }
    }
}
