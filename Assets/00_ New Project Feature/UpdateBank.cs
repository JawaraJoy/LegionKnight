using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public partial class UpdateBank : Singleton<UpdateBank>
    {
        private readonly Dictionary<GameObject, IFixedUpdater> m_FixedActionTick = new();
        private readonly Dictionary<GameObject, IUpdater> m_UpdateActionTick = new();
        private readonly Dictionary<GameObject, ILateUpdater> m_LateUpdateActionTick = new();

        private bool m_StopAllTicks = false;
        private bool m_StopFixedTicks = false;
        private bool m_StopUpdateTicks = false;
        private bool m_StopLateUpdateTicks = false;
        private void FixedUpdate()
        {
            ProcessFixedActionTick();
        }

        private void Update()
        {
            ProcessUpdateActionTick();
        }
        private void LateUpdate()
        {
            ProcessLateUpdateActionTick();
        }
        public void StopAllTicks(bool stop)
        {
            m_StopAllTicks = stop;
        }
        public void StopFixedTicks(bool stop)
        {
            m_StopFixedTicks = stop;
        }
        public void StopUpdateTicks(bool stop)
        {
            m_StopUpdateTicks = stop;
        }
        public void StopLateUpdateTicks(bool stop)
        {
            m_StopLateUpdateTicks = stop;
        }
        private void ProcessFixedActionTick()
        {
            if (m_StopAllTicks) return;
            if (m_StopFixedTicks) return;
            bool IsAny = m_FixedActionTick.Count > 0;
            if (!IsAny) return;
            foreach (var tick in m_FixedActionTick.Values)
            {
                if (tick.IsActive)
                {
                    tick.FixedTick();
                }
            }
        }
        private void ProcessLateUpdateActionTick()
        {
            if (m_StopAllTicks) return;
            if (m_StopLateUpdateTicks) return;
            bool IsAny = m_LateUpdateActionTick.Count > 0;
            if (!IsAny) return;
            foreach (var tick in m_LateUpdateActionTick.Values)
            {
                if (tick.IsActive)
                {
                    tick.LateTick();
                }
            }
        }
        private void ProcessUpdateActionTick()
        {
            if (m_StopAllTicks) return;
            if (m_StopUpdateTicks) return;
            bool IsAny = m_UpdateActionTick.Count > 0;
            if (!IsAny) return;
            foreach (var tick in m_UpdateActionTick.Values)
            {
                if (tick.IsActive)
                {
                    tick.Tick();
                }
            }
        }

        public void RegisterFixedUpdateTick(GameObject key, IFixedUpdater ticker)
        {
            if (!m_FixedActionTick.ContainsKey(key))
            {
                m_FixedActionTick.Add(key, ticker);
            }
        }
        public void RegisterUpdateTick(GameObject key, IUpdater ticker)
        {
            if (!m_UpdateActionTick.ContainsKey(key))
            {
                m_UpdateActionTick.Add(key, ticker);
            }
        }
        public void RegisterLateUpdateTick(GameObject key, ILateUpdater ticker)
        {
            if (!m_LateUpdateActionTick.ContainsKey(key))
            {
                m_LateUpdateActionTick.Add(key, ticker);
            }
        }
        public void UnregisterFixedUpdateTick(GameObject key)
        {
            if (m_FixedActionTick.ContainsKey(key))
            {
                m_FixedActionTick.Remove(key);
            }
        }
        public void UnregisterUpdateTick(GameObject key)
        {
            if (m_UpdateActionTick.ContainsKey(key))
            {
                m_UpdateActionTick.Remove(key);
            }
        }
        public void UnregisterLateUpdateTick(GameObject key)
        {
            if (m_LateUpdateActionTick.ContainsKey(key))
            {
                m_LateUpdateActionTick.Remove(key);
            }
        }
    }
}
