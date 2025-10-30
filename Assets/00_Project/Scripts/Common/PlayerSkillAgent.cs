using UnityEngine;

namespace LegionKnight
{
    public class PlayerSkillAgent : MonoBehaviour
    {
        public void AddManaOvertime(int add, float time)
        {
            Player.Instance.AddManaOvertime(add, time);
        }
    }
}
