using UnityEngine;

namespace Rush
{
    public class UnitConfigExtention_Reborn : MonoBehaviour
    {

    }
    public abstract partial class UnitConfig : IHasReborn
    {
        [SerializeField]
        private RebornConfig m_RebornConfig;
        public RebornConfig RebornConfig => m_RebornConfig;
    }

    public interface IHasReborn
    {
        public RebornConfig RebornConfig { get; }
    }
}
