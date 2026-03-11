using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class SummonController : MonoBehaviour, IUnitExtension
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

        public void AddChargeToSummon(int chargeAmount)
        {
            foreach (var summon in m_Summoners)
            {
                Unit[] summonerUnits = summon.ActiveSummonedUnits.ToArray();
                foreach (var unit in summonerUnits)
                {
                    if (unit.HasBind(out SkillController skillController))
                    {
                        skillController.AddCharges(unit.Config.Skills, chargeAmount);
                    }
                }
            }
        }
    }
}
