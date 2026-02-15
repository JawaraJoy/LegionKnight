
namespace Rush
{
    public class HealerContext
    {
        private readonly IHealer m_Healer;
        private readonly IDamageable m_Damageable;
        public IHealer Healer => m_Healer;
        public IDamageable Damageable => m_Damageable;
        public HealerContext(IHealer healer, IDamageable damageable)
        {
            m_Healer = healer;
            m_Damageable = damageable;
        }
    }
}
