using Mono.Cecil.Cil;
using MonoMod.Cil;
using IL;
using InControl;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class GearSwitcher : Module
{
    private static bool moduleLoaded;
    private static bool runtimeHooksInstalled;

    private protected override void Load()
    {
        coroutineGeneration++;
        moduleLoaded = true;
        SyncRuntimeHooks();
    }

    private protected override void Unload()
    {
        if (runtimeHooksInstalled)
        {
            RestoreRuntimeState(disableGlobalToggle: false);
        }

        moduleLoaded = false;
        SyncRuntimeHooks();
        savedNailAttackBindings.Clear();
        hasSavedNailAttackBindings = false;
    }

    // The game is only hooked while Gear Switcher is switched on; when off it must not touch anything
    private static void SyncRuntimeHooks()
    {
        bool shouldInstall = moduleLoaded && Settings.Enabled;
        if (shouldInstall == runtimeHooksInstalled)
        {
            return;
        }

        runtimeHooksInstalled = shouldInstall;
        if (shouldInstall)
        {
            On.HeroController.Start += OnHeroStart;
            On.HeroController.CharmUpdate += OnCharmUpdate;
            On.InputHandler.OnGUI += OnInputHandlerOnGUI;
            IL.HeroController.SoulGain += OnSoulGainIL;
            On.HealthManager.TakeDamage += OnEnemyDamaged;
            USceneManager.activeSceneChanged += OnSceneChanged;
            On.BossDoorChallengeUI.HideSequence += OnBossDoorHideSequence;
        }
        else
        {
            On.HeroController.Start -= OnHeroStart;
            On.HeroController.CharmUpdate -= OnCharmUpdate;
            On.InputHandler.OnGUI -= OnInputHandlerOnGUI;
            IL.HeroController.SoulGain -= OnSoulGainIL;
            On.HealthManager.TakeDamage -= OnEnemyDamaged;
            USceneManager.activeSceneChanged -= OnSceneChanged;
            On.BossDoorChallengeUI.HideSequence -= OnBossDoorHideSequence;
        }
    }

    private static void OnHeroStart(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        UpdatePantheonShellBindingState();
        ClearPendingApplies();

        string startupPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        GearPreset? startupPresetData = null;
        if (!string.IsNullOrEmpty(startupPreset) && TryGetPreset(startupPreset, out GearPreset currentPreset))
        {
            startupPresetData = currentPreset;
        }

        if (startupPresetData == null)
        {
            startupPreset = "FullGear";
            if (TryGetPreset(startupPreset, out GearPreset fullGearPreset))
            {
                startupPresetData = fullGearPreset;
                if (!string.Equals(GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset, startupPreset, StringComparison.Ordinal))
                {
                    GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = startupPreset;
                    GodhomeQoL.SaveGlobalSettingsSafe();
                }
            }
        }

        RunStartupShellCleanup(startupPresetData);
        if (IsGloballyEnabled && startupPresetData != null)
        {
            QueueApplyPreset(startupPreset);
            ApplyNailInputImmediate(startupPresetData);
        }

        if (!IsGloballyEnabled)
        {
            ScheduleDisabledBindingsCleanup();
        }

        ScheduleOvercharmedReapply();
        ScheduleNailInputReapply();
        ScheduleShellBindingResync();
    }

    private static void OnCharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        orig(self);

        if (IsApplyingPreset)
        {
            return;
        }

        ReapplyForcedOvercharmed();
    }

    private static void OnInputHandlerOnGUI(On.InputHandler.orig_OnGUI orig, InputHandler self)
    {
        orig(self);
        EnforceNaillessInput();
    }

    private static void OnSoulGainIL(ILContext il)
    {
        ILCursor cursor = new(il);
        if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(11)))
        {
            cursor.EmitDelegate<Func<int, int>>(_ => GetMainSoulGain());
        }
        else
        {
            LogError("[GearSwitcher] HeroController.SoulGain: base constant 11 not found, Main Vessel Soul Gain will not apply.");
        }

        cursor = new ILCursor(il);
        if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(6)))
        {
            cursor.EmitDelegate<Func<int, int>>(_ => GetReserveSoulGain());
        }
        else
        {
            LogError("[GearSwitcher] HeroController.SoulGain: base constant 6 not found, Reserve Vessel Soul Gain will not apply.");
        }
    }

    private static void OnSceneChanged(Scene from, Scene to)
    {
        UpdatePantheonShellBindingState();
    }

    private static IEnumerator OnBossDoorHideSequence(On.BossDoorChallengeUI.orig_HideSequence orig, BossDoorChallengeUI self, bool sendEvent)
    {
        IEnumerator origEnum = orig(self, sendEvent);
        while (origEnum.MoveNext())
        {
            yield return origEnum.Current;
        }

        UpdatePantheonShellBindingState();
    }

    private static void OnEnemyDamaged(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (!IsGloballyEnabled)
        {
            orig(self, hitInstance);
            return;
        }

        string lastPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        if (!TryGetPreset(lastPreset, out GearPreset preset))
        {
            orig(self, hitInstance);
            return;
        }

        int rawNailDamage = Math.Max(-99999, Math.Min(99999, preset.NailDamage));
        if (rawNailDamage >= 0 || !IsNailAttack(hitInstance))
        {
            orig(self, hitInstance);
            return;
        }

        int healAmount = Math.Abs(rawNailDamage);
        if (healAmount <= 0)
        {
            orig(self, hitInstance);
            return;
        }

        hitInstance.DamageDealt = 1;
        self.hp += healAmount + 1;
        orig(self, hitInstance);
    }

    private static bool IsNailAttack(HitInstance hitInstance) =>
        hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam;
}
