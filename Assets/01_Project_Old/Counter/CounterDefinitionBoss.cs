using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Counter", menuName = "Legion Knight/CounterBoss", order = 1)]
    public class CounterDefinitionBoss : CounterDefinition
    {
        Dictionary<string, bool> bossCounter = new Dictionary<string, bool>();

        public void AddBoss(BosDefinition bosDefinition)
        {
            if(!bossCounter.ContainsKey(bosDefinition.Id))
            {
                bossCounter.Add(bosDefinition.Id, true);
                base.AddCount(1);
            }
        }

        public void AddBoss(BosDefinition bosDefinition, string id)
        {
            if(bosDefinition.Id.Equals(id))
            {
                AddBoss(bosDefinition);
            }
        }

        public override void ResetCount()
        {
            bossCounter.Clear();
            base.ResetCount();
        }
    }
}
