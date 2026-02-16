using UnityEngine;

namespace Rush
{
    public partial interface IUnitExtension : IHasModuleContext
    {
        void Init(Unit unit);
    }
}
