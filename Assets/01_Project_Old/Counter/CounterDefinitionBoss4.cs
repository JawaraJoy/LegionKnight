using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Counter", menuName = "Legion Knight/CounterBoss4", order = 1)]
    public class CounterDefinitionBoss4 : CounterDefinition
    {
        public void AddBoss(BosDefinition bosDefinition)
        {
            if(bosDefinition.Id.Equals("bos4"))
            {
                base.AddCount(1);
            }
        }
    }
}
