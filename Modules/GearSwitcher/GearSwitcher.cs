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
    public override bool DefaultEnabled => false;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;
    private static readonly int[] PaleCourtCharmIds = { 41, 42, 43, 44 };
    private const string ShowBoundNailEvent = "SHOW BOUND NAIL";
    private const string HideBoundNailEvent = "HIDE BOUND NAIL";
    private const string BindVesselOrbEvent = "BIND VESSEL ORB";
    private const string UnbindVesselOrbEvent = "UNBIND VESSEL ORB";
    private const string MPLoseEvent = "MP LOSE";
    private const string MPReserveUpEvent = "MP RESERVE UP";
    private const string MPReserveDownEvent = "MP RESERVE DOWN";
    private const string UpdateBlueHealthEvent = "UPDATE BLUE HEALTH";
    private static bool pantheonActive;
    private static bool pantheonShellBound;
    private static bool pendingApply;
    private static string pendingPresetName = string.Empty;
    private static bool pendingSpellsApply;
    private static bool pendingNailArtsApply;
    private static bool pendingAbilitiesApply;
    private static bool pendingDreamNailApply;
    private static bool pendingBindingsApply;
    private static bool pendingStatsApply;
    private static bool pendingCharmCostApply;
    private static bool pendingNailInputApply;
    private static bool pendingOvercharmedApply;
    private static GearPreset? pendingSpellsPreset;
    private static GearPreset? pendingNailArtsPreset;
    private static GearPreset? pendingAbilitiesPreset;
    private static GearPreset? pendingDreamNailPreset;
    private static GearPreset? pendingBindingsPreset;
    private static GearPreset? pendingStatsPreset;
    private static GearPreset? pendingCharmCostPreset;
    private static GearPreset? pendingNailInputPreset;
    private static GearPreset? pendingOvercharmedPreset;
    private static StatsPart pendingStatsParts;
    internal static bool IsApplyingPreset { get; private set; }

    [Flags]
    internal enum StatsPart
    {
        None = 0,
        MaxHealth = 1,
        SoulVessels = 2,
        CharmSlots = 4,
        NailDamage = 8,
        All = MaxHealth | SoulVessels | CharmSlots | NailDamage
    }
    private static bool pendingCoroutineRunning;
    private static int coroutineGeneration;
    private static int bindingHudResyncToken;
    private static int soulBindingHudResyncToken;
    private static int healthHudRefreshToken;
    private static bool bindingHudResyncSoulPending;
    private static readonly List<BindingSource> savedNailAttackBindings = new();
    private static bool hasSavedNailAttackBindings;

        internal static GearSwitcherSettings Settings => GodhomeQoL.GlobalSettings.GearSwitcher ??= new GearSwitcherSettings();

        internal static bool IsGloballyEnabled
        {
            get => Settings.Enabled;
            set
            {
                if (Settings.Enabled == value)
                {
                    return;
                }

                Settings.Enabled = value;
                SyncRuntimeHooks();
                GodhomeQoL.SaveGlobalSettingsSafe();
            }
        }


    private static void TryRefreshNailDamage()
    {
        try
        {
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.cs");
        }
    }
}
