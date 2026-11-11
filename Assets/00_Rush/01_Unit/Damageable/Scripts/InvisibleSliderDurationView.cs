using UnityEngine;

namespace Rush
{
    public class InvisibleSliderDurationView : SliderView
    {
        [SerializeField]
        private string m_InvisibleText = "Invisible";

        protected override string GetSliderText(float current, float max)
        {
            return $"{m_InvisibleText}({current:0.#}s)";
        }
    }
}
