
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

            if (unit.Config is BossUnitConfig)
            {
                PlatformHandler handler = RushGameManager.Instance.StageManager.PlatformHandler;
                handler.TouchDownCheckField.OnNormalTouchDown.RemoveListener((skill) => AddChargeToSummonInternal(1));
                handler.TouchDownCheckField.OnPerfectTouchDown.RemoveListener((skill) => AddChargeToSummonInternal(1));

                handler.TouchDownCheckField.OnNormalTouchDown.AddListener((skill) => AddChargeToSummonInternal(10));
                handler.TouchDownCheckField.OnPerfectTouchDown.AddListener((skill) => AddChargeToSummonInternal(10));
            }
        }

        public void AddChargeToSummon(int chargeAmount)
        {
            AddChargeToSummonInternal(chargeAmount);
        }

        private void AddChargeToSummonInternal(int chargeAmount)
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

        public void ResetProgression()
        {
            foreach (var summon in m_Summoners)
            {
                summon.ResetProgression();
            }
        }
    }
}
