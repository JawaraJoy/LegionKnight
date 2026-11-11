# LegionKnight namespace audit

Generated: 2026-11-05
Active scene: Assets/00_Rush/SceneController/Scenes/Aether_Home (Used).unity

## Summary

- namespace LegionKnight scripts: 333
- Referenced by serialized assets/scenes/prefabs: 244
- Referenced by active scene dependency graph: 200
- Unused candidates: 25
- Explicitly deprecated ([Obsolete]): 5

## Unused candidates

These have no serialized asset dependency and no textual type-name reference in project C# sources. Treat as candidates only; reflection, Addressables, or string-based loading may not be visible to this scan.

- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/3rd Party Integrations/Appflyer/AppsFlyerEventAgent.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/3rd Party Integrations/Appflyer/AppsFlyerEventManager.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Character/LevelCharacterAgent.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Currency/Loots/LootObject.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Editor/UnUsedAssetWindow.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Google PlayGames/Save/SaveHandle.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/LauncherProjectile/LauncherController.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/LauncherProjectile/Projectile2D.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/LauncherProjectile/Projectile3D.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Mail/MailAction.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Mail/MailBoxMonitor.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/PlayerInfo/BadgeEventManager.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Progression/PlayerProgressionAgent.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Progression/UnlockedRewardView.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Scripts/Common/GameSettingDefinition.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Scripts/SceneManager/InitScene.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Scripts/UI/PlatformDeckView.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Scripts/UI/PlayButtonView.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Scripts/UI/ResultView.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Timer/DateCountUI.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Timer/WeeklyTimerDefinition.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Unity Service/Ads/AdsAgent.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Unity Service/Ads/IronSourceMediation.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/Legacy/Unity Service/Ads/UnityAdsAgent.cs
- [UNUSED-CANDIDATE] Assets/00_Rush/UI/Name/NameHandler.cs

## Explicitly deprecated

- [DEPRECATED] Assets/00_Rush/00_LocalSave/LocalSaveManager.cs — dependency of Aether_Home; migrate before removal.
- [DEPRECATED] Assets/00_Rush/Legacy/Scripts/Common/Player.cs — dependency of Aether_Home; migrate before removal.
- [DEPRECATED] Assets/00_Rush/Legacy/Scripts/SceneManager/SceneHandler.cs
- [DEPRECATED] Assets/00_Rush/Legacy/Scripts/UI/SkillContainerView.cs
- [DEPRECATED] Assets/00_Rush/Legacy/Unity Service/Cloud Save/CloudSave.cs

## Scope rule

Legacy LegionKnight classes were audited read-only. No class was deleted or modified by this pass. Architecture repairs are applied only to Rush namespace scripts unless legacy edit scope is explicitly authorized.
