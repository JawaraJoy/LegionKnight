using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LootStorage : MonoBehaviour
    {
        [Header("Stored Loots")]
        [SerializeField]
        private List<LootField> m_Looteds = new();

        [SerializeField]
        private List<LootField> m_MirrorLoots = new();

        [Header("Loot Events")]
        [SerializeField]
        private UnityEvent<LootField> m_OnAddNewLoot;

        [SerializeField]
        private UnityEvent<LootField> m_OnLootUpdate;

        [SerializeField]
        private UnityEvent<LootField> m_OnMirrorLootUpdate;

        [SerializeField]
        private UnityEvent<LootField> m_OnLootAmountUpdate;

        [SerializeField]
        private UnityEvent<LootField> m_OnRemoveLoot;

        [SerializeField]
        private UnityEvent<List<LootField>> m_OnTakeLoots;

        [SerializeField]
        private UnityEvent<List<LootField>> m_OnMirrorLootsChanged;

        [SerializeField]
        private UnityEvent<List<LootField>> m_OnLootedsChanged;

        [Header("Direct Take Mode")]
        [SerializeField]
        private bool m_AutoTakeDirectLoot = false;

        [SerializeField]
        private UnityEvent<LootField> m_OnDirectTakeLoot;

        [Header("Transfer Settings")]
        [SerializeField, Min(0f)]
        private float m_MirrorTransferDelayPerUnit = 0.05f;

        [Header("Double Reward Settings")]
        [SerializeField, Min(1)]
        private int m_DoubleRewardSteps = 6;

        [SerializeField, Min(0.01f)]
        private float m_DoubleRewardDuration = 0.2f;

        [SerializeField]
        private UnityEvent m_OnDoubleLootStarted;

        [SerializeField]
        private UnityEvent m_OnDoubleLootFinished;

        public IReadOnlyList<LootField> Looteds => m_Looteds;
        public IReadOnlyList<LootField> MirrorLoots => m_MirrorLoots;
        public bool IsTransferring => m_IsTransferring;

        public UnityEvent<LootField> OnAddNewLootEvent => m_OnAddNewLoot;
        public UnityEvent<LootField> OnLootUpdateEvent => m_OnLootUpdate;
        public UnityEvent<LootField> OnMirrorLootUpdateEvent => m_OnMirrorLootUpdate;
        public UnityEvent<LootField> OnLootAmountUpdateEvent => m_OnLootAmountUpdate;
        public UnityEvent<LootField> OnRemoveLootEvent => m_OnRemoveLoot;
        public UnityEvent<List<LootField>> OnTakeLootsEvent => m_OnTakeLoots;
        public UnityEvent<List<LootField>> OnMirrorLootsChangedEvent => m_OnMirrorLootsChanged;
        public UnityEvent<List<LootField>> OnLootedsChangedEvent => m_OnLootedsChanged;
        public UnityEvent OnDoubleLootStartedEvent => m_OnDoubleLootStarted;
        public UnityEvent OnDoubleLootFinishedEvent => m_OnDoubleLootFinished;

        private bool m_IsTransferring = false;
        private Coroutine m_DoubleRoutine;
        private Coroutine m_TransferRoutine;

        public void TakeLooteds()
        {
            TakeLootedsInternal();
        }

        protected virtual void TakeLootedsInternal()
        {
            if (m_IsTransferring || m_Looteds.Count < 1)
            {
                return;
            }

            List<LootField> lootsSnapshot = new(m_Looteds.Count);
            for (int i = 0; i < m_Looteds.Count; i++)
            {
                lootsSnapshot.Add(m_Looteds[i].Clone());
            }

            for (int i = 0; i < lootsSnapshot.Count; i++)
            {
                lootsSnapshot[i].DirectTakeLoot();
            }

            m_OnTakeLoots?.Invoke(lootsSnapshot);
            ClearLootsInternal();
        }

        public void DirectTakeLoot(LootField loot)
        {
            DirectTakeLootInternal(loot);
        }

        protected virtual void DirectTakeLootInternal(LootField loot)
        {
            if (!m_AutoTakeDirectLoot || loot == null)
            {
                return;
            }

            loot.DirectTakeLoot();
            m_OnDirectTakeLoot?.Invoke(loot);
        }

        public void AddLoots(LootField[] loots)
        {
            AddLootsInternal(loots);
        }

        protected virtual void AddLootsInternal(LootField[] loots)
        {
            if (loots == null || loots.Length == 0)
            {
                return;
            }

            for (int i = 0; i < loots.Length; i++)
            {
                AddLootInternal(loots[i], true);
            }

            m_OnLootedsChanged?.Invoke(m_Looteds);
        }

        public void AddLoot(LootField loot)
        {
            AddLootInternalWrapper(loot);
        }

        protected virtual void AddLootInternalWrapper(LootField loot)
        {
            if (loot == null)
            {
                return;
            }

            AddLootInternal(loot, true);
            m_OnLootedsChanged?.Invoke(m_Looteds);
        }

        private void AddLootInternal(LootField incomingLoot, bool invokeDirectTake)
        {
            if (incomingLoot == null || incomingLoot.ItemLoot == null || incomingLoot.Amount <= 0)
            {
                return;
            }

            LootField existingLoot = GetLootedInternal(incomingLoot.ItemLoot);

            if (existingLoot != null)
            {
                if (incomingLoot.ItemLoot.CollectibleField.IsUnique)
                {
                    m_OnLootUpdate?.Invoke(existingLoot);

                    if (invokeDirectTake)
                    {
                        DirectTakeLootInternal(incomingLoot);
                    }
                    return;
                }

                existingLoot.AddAmount(incomingLoot.Amount);
                m_OnLootAmountUpdate?.Invoke(existingLoot);
                m_OnLootUpdate?.Invoke(existingLoot);

                if (invokeDirectTake)
                {
                    DirectTakeLootInternal(incomingLoot);
                }
                return;
            }

            LootField newLoot = incomingLoot.Clone();
            m_Looteds.Add(newLoot);

            m_OnAddNewLoot?.Invoke(newLoot);
            m_OnLootUpdate?.Invoke(newLoot);

            if (invokeDirectTake)
            {
                DirectTakeLootInternal(incomingLoot);
            }
        }

        public void RemoveLoot(LootField loot)
        {
            RemoveLootInternalWrapper(loot);
        }

        protected virtual void RemoveLootInternalWrapper(LootField loot)
        {
            if (loot == null || m_IsTransferring)
            {
                return;
            }

            RemoveLootInternal(loot);
            m_OnLootedsChanged?.Invoke(m_Looteds);
        }

        private void RemoveLootInternal(LootField loot)
        {
            LootField existingLoot = GetLootedInternal(loot.ItemLoot);
            if (existingLoot == null)
            {
                return;
            }

            m_Looteds.Remove(existingLoot);
            m_OnRemoveLoot?.Invoke(existingLoot);
        }

        public void ClearLoots()
        {
            ClearLootsWrapper();
        }

        protected virtual void ClearLootsWrapper()
        {
            if (m_IsTransferring)
            {
                return;
            }

            ClearLootsInternal();
        }

        private void ClearLootsInternal()
        {
            m_Looteds.Clear();
            m_OnLootedsChanged?.Invoke(m_Looteds);
        }

        public void CopyMirrorFromLooted()
        {
            CopyMirrorFromLootedInternal();
        }

        protected virtual void CopyMirrorFromLootedInternal()
        {
            if (m_IsTransferring)
            {
                return;
            }

            m_MirrorLoots.Clear();

            for (int i = 0; i < m_Looteds.Count; i++)
            {
                m_MirrorLoots.Add(m_Looteds[i].Clone());
            }

            m_OnMirrorLootsChanged?.Invoke(m_MirrorLoots);
        }

        public void StartTransferMirrorToLooteds()
        {
            StartTransferMirrorToLootedsInternal();
        }

        protected virtual void StartTransferMirrorToLootedsInternal()
        {
            if (!gameObject.activeInHierarchy)
            {
                TransferMirrorToLootedsImmediateInternal();
                return;
            }

            if (m_TransferRoutine != null)
            {
                RushGameManager.Instance.StopCoroutine(m_TransferRoutine);
            }

            m_TransferRoutine = RushGameManager.Instance.StartCoroutine(TransferMirrorToLootedsRoutine());
        }

        public void TransferMirrorToLootedsImmediate()
        {
            TransferMirrorToLootedsImmediateInternal();
        }

        protected virtual void TransferMirrorToLootedsImmediateInternal()
        {
            if (m_MirrorLoots.Count < 1)
            {
                return;
            }

            if (m_IsTransferring)
            {
                return;
            }

            m_IsTransferring = true;

            List<LootField> mirrorsCopy = new(m_MirrorLoots.Count);
            for (int i = 0; i < m_MirrorLoots.Count; i++)
            {
                mirrorsCopy.Add(m_MirrorLoots[i].Clone());
            }

            for (int i = 0; i < mirrorsCopy.Count; i++)
            {
                LootField mirrorLoot = mirrorsCopy[i];
                if (mirrorLoot.Amount <= 0)
                {
                    continue;
                }

                AddLootInternal(mirrorLoot, false);
            }

            m_MirrorLoots.Clear();
            m_IsTransferring = false;

            m_OnLootedsChanged?.Invoke(m_Looteds);
            m_OnMirrorLootsChanged?.Invoke(m_MirrorLoots);
        }

        private IEnumerator TransferMirrorToLootedsRoutine()
        {
            if (m_IsTransferring || m_MirrorLoots.Count < 1)
            {
                yield break;
            }

            m_IsTransferring = true;
            yield return null;

            List<LootField> mirrorsCopy = new(m_MirrorLoots.Count);
            for (int i = 0; i < m_MirrorLoots.Count; i++)
            {
                mirrorsCopy.Add(m_MirrorLoots[i].Clone());
            }

            for (int i = 0; i < mirrorsCopy.Count; i++)
            {
                LootField mirrorLoot = mirrorsCopy[i];
                int count = mirrorLoot.Amount;

                for (int unit = 0; unit < count; unit++)
                {
                    mirrorLoot.AddAmount(-1);
                    m_OnMirrorLootUpdate?.Invoke(mirrorLoot);

                    LootField oneLoot = new LootField(mirrorLoot.ItemLoot, 1, mirrorLoot.Chance);
                    AddLootInternal(oneLoot, false);

                    m_OnLootedsChanged?.Invoke(m_Looteds);

                    if (m_MirrorTransferDelayPerUnit > 0f)
                    {
                        yield return new WaitForSeconds(m_MirrorTransferDelayPerUnit);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }

            m_MirrorLoots.Clear();
            m_IsTransferring = false;

            m_OnLootedsChanged?.Invoke(m_Looteds);
            m_OnMirrorLootsChanged?.Invoke(m_MirrorLoots);
            m_TransferRoutine = null;
        }

        public void StartDoubleStoredLoots()
        {
            StartDoubleStoredLootsInternal();
        }

        protected virtual void StartDoubleStoredLootsInternal()
        {
            if (m_IsTransferring || m_Looteds.Count < 1)
            {
                return;
            }

            if (!gameObject.activeInHierarchy)
            {
                DoubleStoredLootsImmediateInternal();
                return;
            }

            if (m_DoubleRoutine != null)
            {
                RushGameManager.Instance.StopCoroutine(m_DoubleRoutine);
            }

            m_DoubleRoutine = RushGameManager.Instance.StartCoroutine(DoubleStoredLootsRoutine());
        }

        public void DoubleStoredLootsImmediate()
        {
            DoubleStoredLootsImmediateInternal();
        }

        protected virtual void DoubleStoredLootsImmediateInternal()
        {
            if (m_IsTransferring || m_Looteds.Count < 1)
            {
                return;
            }

            m_IsTransferring = true;
            m_OnDoubleLootStarted?.Invoke();

            for (int i = 0; i < m_Looteds.Count; i++)
            {
                LootField loot = m_Looteds[i];
                loot.SetAmount(loot.Amount * 2);
                m_OnLootAmountUpdate?.Invoke(loot);
                m_OnLootUpdate?.Invoke(loot);
            }

            m_OnLootedsChanged?.Invoke(m_Looteds);
            m_OnDoubleLootFinished?.Invoke();
            m_IsTransferring = false;
        }

        private IEnumerator DoubleStoredLootsRoutine()
        {
            if (m_IsTransferring || m_Looteds.Count < 1)
            {
                yield break;
            }

            m_IsTransferring = true;
            m_OnDoubleLootStarted?.Invoke();

            int count = m_Looteds.Count;
            int steps = Mathf.Max(1, m_DoubleRewardSteps);
            float duration = Mathf.Max(0.01f, m_DoubleRewardDuration);
            float waitPerStep = duration / steps;

            int[] originalAmounts = new int[count];
            int[] bonusAmounts = new int[count];
            int[] appliedBonus = new int[count];

            for (int i = 0; i < count; i++)
            {
                originalAmounts[i] = m_Looteds[i].Amount;
                bonusAmounts[i] = originalAmounts[i];
                appliedBonus[i] = 0;
            }

            for (int step = 1; step <= steps; step++)
            {
                bool anyChanged = false;

                for (int i = 0; i < count; i++)
                {
                    int shouldApply = Mathf.RoundToInt((float)bonusAmounts[i] * step / steps);
                    int delta = shouldApply - appliedBonus[i];

                    if (delta == 0)
                    {
                        continue;
                    }

                    appliedBonus[i] += delta;
                    m_Looteds[i].AddAmount(delta);

                    m_OnLootAmountUpdate?.Invoke(m_Looteds[i]);
                    m_OnLootUpdate?.Invoke(m_Looteds[i]);
                    anyChanged = true;
                }

                if (anyChanged)
                {
                    m_OnLootedsChanged?.Invoke(m_Looteds);
                }

                if (step < steps)
                {
                    yield return new WaitForSeconds(waitPerStep);
                }
            }

            m_IsTransferring = false;
            m_OnDoubleLootFinished?.Invoke();
            m_DoubleRoutine = null;
        }

        protected virtual LootField GetLootedInternal(CollectibleConfig collectibleConfig)
        {
            return m_Looteds.Find(x => x.ItemLoot == collectibleConfig);
        }

        protected virtual bool HasLootedInternal(CollectibleConfig collectibleConfig)
        {
            return GetLootedInternal(collectibleConfig) != null;
        }
    }
}