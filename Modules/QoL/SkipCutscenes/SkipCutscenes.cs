using Vasi;
using Random = UnityEngine.Random;

namespace GodhomeQoL.Modules.QoL
{

    [UsedImplicitly]
    public sealed partial class SkipCutscenes : Module
    {
        #region Settings

        [GlobalSetting]
        public static bool AbsoluteRadiance = false;

        [GlobalSetting]
        public static bool HallOfGodsStatues = false;

        [GlobalSetting]
        public static bool PureVesselRoar = false;

        [GlobalSetting]
        public static bool GrimmNightmare = false;

        [GlobalSetting]
        public static bool GreyPrinceZote = false;

        [GlobalSetting]
        public static bool Collector = false;

        [GlobalSetting]
        public static bool AutoSkipCinematics = false;

        [GlobalSetting]
        public static bool AllowSkippingNonskippable = false;

        [GlobalSetting]
        public static bool SkipCutscenesWithoutPrompt = false;

        [GlobalSetting]
        public static bool SoulMasterPhaseTransitionSkip = false;

        [GlobalSetting]
        public static bool PantheonVEnding = false;

        #endregion
        public override bool DefaultEnabled => false;
        public override bool Hidden => true;
        public override bool AlwaysEnabled => true;

        public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

        private static readonly (Func<bool>, Func<Scene, IEnumerator>)[] FSM_SKIPS =
        {
            (() => AbsoluteRadiance, AbsRadSkip),
            (() => PureVesselRoar, HKPrimeSkip),
            (() => GrimmNightmare, GrimmNightmareSkip),
            (() => GreyPrinceZote, GreyPrinceZoteSkip),
            (() => Collector, CollectorSkip),
            (() => PantheonVEnding, PantheonVEndingSkip),
            (() => HallOfGodsStatues, StatueWait)
        };

        private static readonly HashSet<string> GodhomeHubScenes = new(StringComparer.Ordinal)
        {
            "GG_Workshop",
            "GG_Atrium",
            "GG_Atrium_Roof"
        };
        private static readonly HashSet<string> PantheonLikeScenes = new(StringComparer.OrdinalIgnoreCase)
        {
            "gg dryya",
            "gg hegemol",
            "gg zemer",
            "gg isma"
        };
        private static readonly HashSet<string> BossAnimationSceneNames = new(StringComparer.Ordinal)
        {
            "GG_Radiance",
            "GG_Hollow_Knight",
            "GG_Grimm_Nightmare",
            "GG_Grey_Prince_Zote",
            "GG_Collector",
            "GG_Collector_V",
            "GG_Mage_Knight",
            "GG_Mage_Knight_V"
        };

        private static bool suppressAutoSkipForTransition;
        private static bool timeScaleOverrideActive;
        private static int timeScaleOverrideHandle;
        private static int timeScaleOverrideGeneration;
        private const int StatuePatchMaxRetryFrames = 45;
        private const float StatueApproachMaxWait = 0.06f;
        private const float StatueDreamBoxDownMaxWait = 0.08f;
        private const string AbsoluteRadianceScene = "GG_Radiance";
    }
}
