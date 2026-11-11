using LegionKnight;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class UpdateBank : Singleton<UpdateBank>
    {
        private readonly Dictionary<GameObject, IFixedUpdater> m_FixedActionTick = new();
        private readonly List<IFixedUpdater> m_FixedSnapshot = new();
        private readonly Dictionary<GameObject, IUpdater> m_UpdateActionTick = new();
        private readonly List<IUpdater> m_UpdateSnapshot = new();
        private readonly Dictionary<GameObject, ILateUpdater> m_LateUpdateActionTick = new();
        private readonly List<ILateUpdater> m_LateSnapshot = new();

        private bool m_StopAllTicks = false;
        private bool m_StopFixedTicks = false;
        private bool m_StopUpdateTicks = false;
        private bool m_StopLateUpdateTicks = false;

private void Start()
        {
            SubscribeCanvasEvents();
        }
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
            StopAllTicksInternal(stop);
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
            if (m_StopAllTicks || m_StopFixedTicks || m_FixedActionTick.Count == 0)
            {
                return;
            }

            m_FixedSnapshot.Clear();
            m_FixedSnapshot.AddRange(m_FixedActionTick.Values);
            foreach (IFixedUpdater tick in m_FixedSnapshot)
            {
                if (tick != null && !(tick is Object unityObject && unityObject == null) && tick.IsActive)
                {
                    tick.FixedTick();
                }
            }
        }
private void ProcessLateUpdateActionTick()
        {
            if (m_StopAllTicks || m_StopLateUpdateTicks || m_LateUpdateActionTick.Count == 0)
            {
                return;
            }

            m_LateSnapshot.Clear();
            m_LateSnapshot.AddRange(m_LateUpdateActionTick.Values);
            foreach (ILateUpdater tick in m_LateSnapshot)
            {
                if (tick != null && !(tick is Object unityObject && unityObject == null) && tick.IsActive)
                {
                    tick.LateTick();
                }
            }
        }
private void ProcessUpdateActionTick()
        {
            if (m_StopAllTicks || m_StopUpdateTicks || m_UpdateActionTick.Count == 0)
            {
                return;
            }

            m_UpdateSnapshot.Clear();
            m_UpdateSnapshot.AddRange(m_UpdateActionTick.Values);
            foreach (IUpdater tick in m_UpdateSnapshot)
            {
                if (tick != null && !(tick is Object unityObject && unityObject == null) && tick.IsActive)
                {
                    tick.Tick();
                }
            }
        }

public void RegisterFixedUpdateTick(GameObject key, IFixedUpdater ticker)
        {
            if (key != null && ticker != null && !m_FixedActionTick.ContainsKey(key))
            {
                m_FixedActionTick.Add(key, ticker);
            }
        }
public void RegisterUpdateTick(GameObject key, IUpdater ticker)
        {
            if (key != null && ticker != null && !m_UpdateActionTick.ContainsKey(key))
            {
                m_UpdateActionTick.Add(key, ticker);
            }
        }
public void RegisterLateUpdateTick(GameObject key, ILateUpdater ticker)
        {
            if (key != null && ticker != null && !m_LateUpdateActionTick.ContainsKey(key))
            {
                m_LateUpdateActionTick.Add(key, ticker);
            }
        }
public void UnregisterFixedUpdateTick(GameObject key)
        {
            if (key != null)
            {
                m_FixedActionTick.Remove(key);
            }
        }
public void UnregisterUpdateTick(GameObject key)
        {
            if (key != null)
            {
                m_UpdateActionTick.Remove(key);
            }
        }
public void UnregisterLateUpdateTick(GameObject key)
        {
            if (key != null)
            {
                m_LateUpdateActionTick.Remove(key);
            }
        }
    

private void StopAllTicksInternal(bool stop)
        {
            m_StopAllTicks = stop;
        }


private void HandleNoMainPanelShow()
        {
            StopAllTicksInternal(false);
        }


private void HandleAnyMainPanelShow()
        {
            StopAllTicksInternal(true);
        }


private void UnsubscribeCanvasEvents()
        {
            if (CanvasManager.Instance == null || CanvasManager.Instance.MainCanvas == null)
            {
                return;
            }

            var mainCanvas = CanvasManager.Instance.MainCanvas;
            mainCanvas.OnAnyMainPanelShow.RemoveListener(HandleAnyMainPanelShow);
            mainCanvas.OnNoMainPanelShow.RemoveListener(HandleNoMainPanelShow);
        }


private void SubscribeCanvasEvents()
        {
            if (CanvasManager.Instance == null || CanvasManager.Instance.MainCanvas == null)
            {
                return;
            }

            var mainCanvas = CanvasManager.Instance.MainCanvas;
            mainCanvas.OnAnyMainPanelShow.RemoveListener(HandleAnyMainPanelShow);
            mainCanvas.OnNoMainPanelShow.RemoveListener(HandleNoMainPanelShow);
            mainCanvas.OnAnyMainPanelShow.AddListener(HandleAnyMainPanelShow);
            mainCanvas.OnNoMainPanelShow.AddListener(HandleNoMainPanelShow);
        }


private void OnDisable()
        {
            UnsubscribeCanvasEvents();
        }


private void OnEnable()
        {
            SubscribeCanvasEvents();
        }
}
}
