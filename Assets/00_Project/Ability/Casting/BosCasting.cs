using UnityEngine;

namespace LegionKnight
{
    public class BosCasting : Casting
    {
        private void OnEnable()
        {
            GameManager.Instance.OnBosDeath.AddListener(OnBosDeath);
        }
        private void OnDisable()
        {
            GameManager.Instance.OnBosDeath.RemoveListener(OnBosDeath);
        }

        private void OnBosDeath()
        {
            StopCasting();
            ResetCastingTime();
        }
    }
}
