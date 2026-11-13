using UnityEngine;

namespace Rush
{
    // Compatibility entrypoint for existing project callers and serialized integrations.
    public class CollectibleControl : MonoBehaviour
    {
        public static void AddCollectibleStatic(string source, CollectibleConfig config, int amount)
        {
            if (config == null || amount <= 0) throw new System.ArgumentException("Invalid collectible reward.");
            Debug.Log($"[CollectibleControl] +{amount}x {config.name}");
            if (config is LegionKnight.CustomImageDefinition cosmetic)
            {
                var profile = LegionKnight.Player.Instance.CustomProfile;
                var entry = cosmetic.Type == LegionKnight.CustomImageType.Icon
                    ? profile.GetIcon(cosmetic)
                    : profile.GetFrame(cosmetic);
                if (entry == null)
                    throw new System.InvalidOperationException($"Profile reward {cosmetic.name} is missing from the catalog.");
                if (!entry.Owned)
                {
                    // Use the profile's existing save records. The claim popup owns the presentation.
                    var save = LegionKnight.UnityService.Instance;
                    string id = cosmetic.BaseInfo.Id;
                    save.LocalSave.ExecuteTransaction(() =>
                    {
                        save.SaveData("owned" + id, true);
                        save.SaveData("con" + id, ProductCondition.NewUnlocked, callback: () =>
                        {
                            entry.Init();
                            if (profile != null) profile.Refres();
                        });
                    });
                }
            }
            LootField.CharacterApplier(config);
            LootField.CurrencyApplier(config, amount);
            LootField.CardApplier(config, amount);
            LootField.EnergyApplier(config, amount);

            // Analytics must never turn a successful inventory grant into a failed transaction.
            if (!Application.isPlaying) return;
            try { config.OnCollect(source, amount); }
            catch (System.Exception ex) { Debug.LogWarning($"[CollectibleControl] Analytics failed: {ex.Message}"); }
        }
    }
}