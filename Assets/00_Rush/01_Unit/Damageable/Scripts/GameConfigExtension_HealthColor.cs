using UnityEngine;

namespace Rush
{
    public class GameConfigExtension_HealthColor
    {

    }
    [System.Serializable]
    public struct HealthColorConfig
    {
        [SerializeField]
        private Color HealthyColour;
        [SerializeField]
        private Color MiddleHealthColor;
        [SerializeField]
        private Color LowHealthColor;

        public readonly Color GetHealthColor(float healthRate)
        {
            if (healthRate > 0.5f)
            {
                return HealthyColour;
            }
            else if (healthRate > 0.25f)
            {
                return MiddleHealthColor;
            }
            else
            {
                return LowHealthColor;
            }
        }
    }
    public partial class GameConfig
    {
        [SerializeField]
        private HealthColorConfig m_HealthColorConfig;
        public HealthColorConfig HealthColorConfig => m_HealthColorConfig;
    }
}
