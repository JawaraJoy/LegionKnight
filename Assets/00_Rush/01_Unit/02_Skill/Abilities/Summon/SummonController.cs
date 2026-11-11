using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class SummonController : MonoBehaviour, IUnitExtension, IReseter
    {
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        [SerializeField]
        private List<Summoner> m_Summoners = new List<Summoner>();
        public List<Summoner> Summoners => m_Summoners;

        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
        }

        private void OnDisable() => ResetProgressionInternal();

        public void AddChargeToSummon(int chargeAmount)
        {
            AddChargeToSummonInternal(chargeAmount);
        }

        private void AddChargeToSummonInternal(int chargeAmount)
        {
            Unit owner = m_ModuleContext?.Unit;
            if (!isActiveAndEnabled || owner == null || !owner.gameObject.activeInHierarchy ||
                (owner.HasBind(out Damageable ownerHealth) && !ownerHealth.IsAlive)) return;

            foreach (var summon in m_Summoners.ToArray())
            {
                if (summon == null || !summon.isActiveAndEnabled) continue;
                Unit[] summonerUnits = summon.ActiveSummonedUnits.ToArray();
                foreach (var unit in summonerUnits)
                {
                    if (unit == null || !unit.gameObject.activeInHierarchy || unit.Config == null ||
                        (unit.HasBind(out Damageable health) && !health.IsAlive)) continue;
                    if (unit.HasBind(out SkillController skillController))
                    {
                        skillController.AddCharges(unit.Config.Skills, chargeAmount);
                    }
                }
            }
        }

        public void ResetProgression() => ResetProgressionInternal();

        private void ResetProgressionInternal()
        {
            foreach (var summon in m_Summoners)
            {
                if (summon != null) summon.ResetProgression();
            }
        }
    }
}
