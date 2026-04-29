namespace Rush
{
    public class GachaRateInfo
    {
        private readonly GachaCollectableConfig m_Collectable;
        private readonly float m_NormalizedChance;

        public GachaCollectableConfig Collectable => m_Collectable;
        public float NormalizedChance => m_NormalizedChance;
        public float Percent => m_NormalizedChance * 100f;

        public GachaRateInfo(GachaCollectableConfig collectable, float normalizedChance)
        {
            m_Collectable = collectable;
            m_NormalizedChance = normalizedChance;
        }
    }
}