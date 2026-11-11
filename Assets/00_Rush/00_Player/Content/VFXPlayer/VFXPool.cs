using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class VFXPool : MonoBehaviour
    {
        public enum SpawnMode
        {
            World,
            Local
        }

        [SerializeField]
        private VfxPLayer m_VfxPrefab;

        [SerializeField]
        private int m_PreWarmCount = 10;

        [SerializeField]
        private bool m_AllowExpand = true;

        [SerializeField]
        private SpawnMode m_SpawnMode = SpawnMode.World;

        private readonly Queue<VfxPLayer> m_Pool = new Queue<VfxPLayer>();
        private readonly HashSet<VfxPLayer> m_Active = new HashSet<VfxPLayer>();

        private void Awake()
        {
            if (m_VfxPrefab == null)
            {
                Debug.LogError($"{name} VFX Prefab not assigned.");
                enabled = false;
                return;
            }

            PreWarm();
        }

        private void PreWarm()
        {
            for (int i = 0; i < m_PreWarmCount; i++)
            {
                var vfx = CreateNewVFX();
                m_Pool.Enqueue(vfx);
            }
        }

        private VfxPLayer CreateNewVFX()
        {
            var vfx = Instantiate(m_VfxPrefab, transform);
            vfx.gameObject.SetActive(false);
            vfx.SetOnFinished(ReturnPool);
            return vfx;
        }

        public void Play()
        {
            VfxPLayer vfx = null;

            if (m_Pool.Count > 0)
            {
                vfx = m_Pool.Dequeue();
            }
            else if (m_AllowExpand)
            {
                vfx = CreateNewVFX();
            }
            else
            {
                return;
            }

            m_Active.Add(vfx);

            SetupTransform(vfx);

            vfx.gameObject.SetActive(true);
            vfx.Play();
        }
        public void Stop()
        {
            foreach (var vfx in m_Active)
            {
                vfx.ForceStop();
            }
        }

        private void SetupTransform(VfxPLayer vfx)
        {
            switch (m_SpawnMode)
            {
                case SpawnMode.World:
                    vfx.transform.SetParent(null);
                    vfx.transform.position = transform.position;
                    vfx.transform.rotation = transform.rotation;
                    break;

                case SpawnMode.Local:
                    vfx.transform.SetParent(transform);
                    vfx.transform.localPosition = Vector3.zero;
                    vfx.transform.localRotation = Quaternion.identity;
                    break;
            }
        }

        private void ReturnPool(VfxPLayer vfx)
        {
            if (!m_Active.Contains(vfx))
                return;

            m_Active.Remove(vfx);

            vfx.gameObject.SetActive(false);
            vfx.transform.SetParent(transform);

            m_Pool.Enqueue(vfx);
        }

        public void ForceStopAll()
        {
            foreach (var vfx in m_Active)
            {
                vfx.ForceStop();
            }
        }
    }
}