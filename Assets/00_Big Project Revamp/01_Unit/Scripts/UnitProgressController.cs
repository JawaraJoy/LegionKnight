
using UnityEngine;

namespace Rush
{
    public class UnitProgressController : MonoBehaviour, IUnitExtension
    {
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;
        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
        }

        public void SetLevel(int level)
        {
            if (m_ModuleContext.Unit != null)
            {
                m_ModuleContext.Unit.Progression.SetLevel(level);
                if (m_ModuleContext.Unit.HasBind(out Damageable damageable))
                {
                    damageable.RefreshDamageableStat(1f, false);
                }
            }
        }
    }
}
