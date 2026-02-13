using UnityEngine;

namespace Rush
{
    public static partial class CommonUtility
    {
        public static bool LayerConfirmation(LayerMask considerLayer, LayerMask targetLayer)
        {
            bool isConfirmed = (considerLayer & (1 << targetLayer)) != 0;
            return isConfirmed;
        }
    }
}
