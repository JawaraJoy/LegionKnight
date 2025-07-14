using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageOvertimeAgent : MonoBehaviour
    {
        public void ApplyPlayerDamageOverTime(int damagePerSecond, float duration)
        {
            Player.Instance.ApplyPlayerDamageOverTime(damagePerSecond, duration);
        }
        public void StopPlayerDamageOverTime()
        {
            Player.Instance.StopPlayerDamageOverTime();
        }
        public void AddAntidot(int count)
        {
            Player.Instance.AddAntidot(count);
        }
    }
}
