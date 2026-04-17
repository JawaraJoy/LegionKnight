namespace Rush
{
    public class GachaResultItemUI : GachaCollectableItemUI
    {
        public void Setup(GachaCollectableConfig collectable)
        {
            SetupBase(collectable);
            OnSetupComplete(collectable);
        }
    }
}