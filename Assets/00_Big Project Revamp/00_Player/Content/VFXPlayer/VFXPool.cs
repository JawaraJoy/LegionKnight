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
        private SpawnMode m_SpawnMode = SpawnMode.World;

        private readonly Queue<VfxPLayer> m_Pool = new Queue<VfxPLayer>();

        private void Awake()
        {
            PreWarm();
        }

        private void PreWarm()
        {
            for (int i = 0; i < m_PreWarmCount; i++)
            {
                VfxPLayer vfx = CreateNewVFX();
                ReturnPool(vfx);
            }
        }

        private VfxPLayer CreateNewVFX()
        {
            VfxPLayer vfx = Instantiate(m_VfxPrefab, transform);
            vfx.gameObject.SetActive(false);
            vfx.SetOnFinished(ReturnPool);
            return vfx;
        }

        public void Play()
        {
            VfxPLayer vfx = m_Pool.Count > 0
                ? m_Pool.Dequeue()
                : CreateNewVFX();

            SetupTransform(vfx);

            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        private void SetupTransform(VfxPLayer vfx)
        {
            switch (m_SpawnMode)
            {
                case SpawnMode.World:
                    vfx.transform.position = transform.position;
                    vfx.transform.rotation = transform.rotation;
                    break;

                case SpawnMode.Local:
                    vfx.transform.localPosition = Vector3.zero;
                    vfx.transform.localRotation = Quaternion.identity;
                    break;
            }
        }

        private void ReturnPool(VfxPLayer vfx)
        {
            vfx.gameObject.SetActive(false);
            m_Pool.Enqueue(vfx);
        }
    }
}
