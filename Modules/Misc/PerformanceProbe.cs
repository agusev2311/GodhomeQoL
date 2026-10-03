using System.Diagnostics;
using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.Misc;

public sealed class PerformanceProbe : Module
{
    [GlobalSetting] public static bool PerformanceProbeEnabled = false;
    [GlobalSetting] public static int PerformanceProbeDurationSeconds = 20;
    [GlobalSetting] public static bool PerformanceProbeVerbose = false;

    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private static bool monitorActive;
    private static float monitorUntilTime;
    private static float nextSummaryTime;
    private static int sessionId;

    private static int sampledFrames;
    private static float sampledTime;
    private static float sampledMaxDelta;
    private static int spikeLogs;

    private static bool heroPresenceKnown;
    private static bool lastHeroPresent;
    private static bool zeroScaleKnown;
    private static bool lastZeroScale;
    private static int missingGameManagerLogCount;
    private static int missingHeroLogCount;
    private static int missingGameManagerLogCountSnapshot;
    private static int missingHeroLogCountSnapshot;
    private static int missingGameManagerStackSamples;
    private static int missingHeroStackSamples;

    private static bool hooksInstalled;

    private protected override void Load()
    {
        // Diagnostics only: stay completely detached unless explicitly enabled
        if (!PerformanceProbeEnabled)
        {
            return;
        }

        hooksInstalled = true;
        USceneManager.activeSceneChanged += OnSceneChanged;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        On.BossSceneController.Start += OnBossSceneStart;
        On.GameManager.SetTimeScale_float += OnSetTimeScale;
        Application.logMessageReceived += OnUnityLog;

        string currentScene = GetCurrentSceneName();
        if (IsBossLikeScene(currentScene))
        {
            ArmMonitor($"load:{currentScene}");
        }
    }

    private protected override void Unload()
    {
        if (!hooksInstalled)
        {
            return;
        }

        hooksInstalled = false;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        On.BossSceneController.Start -= OnBossSceneStart;
        On.GameManager.SetTimeScale_float -= OnSetTimeScale;
        Application.logMessageReceived -= OnUnityLog;

        monitorActive = false;
    }

    private static void OnSceneChanged(Scene from, Scene to)
    {
        if (!PerformanceProbeEnabled)
        {
            return;
        }

        if (IsBossLikeScene(to.name))
        {
            ArmMonitor($"scene:{from.name}->{to.name}");
        }
    }

    private static IEnumerator OnBossSceneStart(On.BossSceneController.orig_Start orig, BossSceneController self)
    {
        bool profiling = PerformanceProbeEnabled;
        Stopwatch? sw = null;
        int frames = 0;

        if (profiling)
        {
            string scene = self.gameObject.scene.name ?? string.Empty;
            ArmMonitor($"boss-start:{scene}");
            sw = Stopwatch.StartNew();
            Log($"[PerfProbe] BossSceneController.Start begin scene={scene}");
        }

        IEnumerator origEnum = orig(self);
        while (origEnum.MoveNext())
        {
            frames++;
            if (profiling && PerformanceProbeVerbose && frames % 120 == 0)
            {
                Log($"[PerfProbe] BossSceneController.Start progress frames={frames}");
            }

            yield return origEnum.Current;
        }

        if (profiling && sw != null)
        {
            sw.Stop();
            Log($"[PerfProbe] BossSceneController.Start end elapsedMs={sw.ElapsedMilliseconds} frames={frames}");
            LogContext("boss-start-end");
        }
    }

    private static void OnSetTimeScale(On.GameManager.orig_SetTimeScale_float orig, GameManager self, float newTimeScale)
    {
        if (PerformanceProbeEnabled && monitorActive && newTimeScale <= 0.001f)
        {
            Log($"[PerfProbe] GameManager.SetTimeScale -> {newTimeScale:0.###} scene={self.sceneName}");
            LogContext("set-timescale-zero");
        }

        orig(self, newTimeScale);
    }

    private static void OnHeroUpdate()
    {
        if (!PerformanceProbeEnabled || !monitorActive)
        {
            return;
        }

        float now = Time.unscaledTime;
        if (now >= monitorUntilTime)
        {
            LogSummary("stop");
            LogContext("monitor-stop");
            monitorActive = false;
            return;
        }

        float dt = Time.unscaledDeltaTime;
        sampledFrames++;
        sampledTime += dt;
        if (dt > sampledMaxDelta)
        {
            sampledMaxDelta = dt;
        }

        bool heroPresent = HeroController.instance != null;
        if (!heroPresenceKnown || heroPresent != lastHeroPresent)
        {
            heroPresenceKnown = true;
            lastHeroPresent = heroPresent;
            Log($"[PerfProbe] Hero {(heroPresent ? "present" : "missing")} frame={Time.frameCount} scene={GetCurrentSceneName()}");
        }

        bool zeroScale = Time.timeScale <= 0.001f || TimeController.GenericTimeScale <= 0.001f;
        if (!zeroScaleKnown || zeroScale != lastZeroScale)
        {
            zeroScaleKnown = true;
            lastZeroScale = zeroScale;
            Log($"[PerfProbe] TimeScale state={(zeroScale ? "ZERO" : "NONZERO")} ts={Time.timeScale:0.###} gts={TimeController.GenericTimeScale:0.###}");
        }

        if (dt >= 0.08f && spikeLogs < 15)
        {
            spikeLogs++;
            Log($"[PerfProbe] Spike dt={dt:0.000}s (~{(dt <= 0f ? 0 : 1f / dt):0.0} fps) frame={Time.frameCount}");
            LogContext("spike");
        }

        if (now >= nextSummaryTime)
        {
            LogSummary("tick");
            nextSummaryTime = now + 1f;
        }
    }

    private static void ArmMonitor(string reason)
    {
        if (!PerformanceProbeEnabled)
        {
            return;
        }

        sessionId++;
        monitorActive = true;
        monitorUntilTime = Time.unscaledTime + Mathf.Clamp(PerformanceProbeDurationSeconds, 5, 120);
        nextSummaryTime = Time.unscaledTime + 1f;
        sampledFrames = 0;
        sampledTime = 0f;
        sampledMaxDelta = 0f;
        spikeLogs = 0;
        heroPresenceKnown = false;
        zeroScaleKnown = false;

        Log($"[PerfProbe] Monitor ON session={sessionId} reason={reason} durationSec={Mathf.Clamp(PerformanceProbeDurationSeconds, 5, 120)}");
        LogContext("monitor-arm");
    }

    private static void LogSummary(string phase)
    {
        if (sampledFrames <= 0 || sampledTime <= 0f)
        {
            return;
        }

        float fps = sampledFrames / Math.Max(0.0001f, sampledTime);
        float avgMs = (sampledTime / sampledFrames) * 1000f;
        float maxMs = sampledMaxDelta * 1000f;
        long managedMb = GC.GetTotalMemory(false) / (1024 * 1024);
        int gmMissDelta = missingGameManagerLogCount - missingGameManagerLogCountSnapshot;
        int heroMissDelta = missingHeroLogCount - missingHeroLogCountSnapshot;
        missingGameManagerLogCountSnapshot = missingGameManagerLogCount;
        missingHeroLogCountSnapshot = missingHeroLogCount;

        Log(
            $"[PerfProbe] {phase} fps={fps:0.0} avgMs={avgMs:0.0} maxMs={maxMs:0.0} frames={sampledFrames} " +
            $"scene={GetCurrentSceneName()} memMB={managedMb} ts={Time.timeScale:0.###} gts={TimeController.GenericTimeScale:0.###} " +
            $"gmMissing+={gmMissDelta} heroMissing+={heroMissDelta}"
        );

        sampledFrames = 0;
        sampledTime = 0f;
        sampledMaxDelta = 0f;
    }

    private static void LogContext(string tag)
    {
        GameManager? gm = GameManager.instance;
        HeroController? hero = HeroController.instance;

        bool inSequence = IsInSequenceSafe();
        bool isBossScene = BossSceneController.IsBossScene;
        int bossLevel = GetBossLevelSafe();

        bool freezeEnabled = FreezeHitboxes.GetEnabled();
        bool freezeAnyHits = FreezeHitboxes.GetAnyHitsMode();

        Log(
            $"[PerfProbe] {tag} scene={GetCurrentSceneName()} gm={(gm == null ? "null" : gm.gameState.ToString())} " +
            $"transition={(gm?.IsInSceneTransition ?? false)} hero={(hero != null)} " +
            $"bossScene={isBossScene} inSequence={inSequence} bossLevel={bossLevel} " +
            $"speedEnabled={SpeedChanger.globalSwitch} speed={SpeedChanger.speed:0.##} " +
            $"freezeEnabled={freezeEnabled} freezeAnyHits={freezeAnyHits}"
        );
    }

    private static bool IsInSequenceSafe()
    {
        try
        {
            return BossSequenceController.IsInSequence;
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "PerformanceProbe.cs");
            return false;
        }
    }

    private static int GetBossLevelSafe()
    {
        try
        {
            BossSceneController? controller = BossSceneController.Instance;
            if (controller == null)
            {
                return -1;
            }

            return controller.Reflect().bossLevel;
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "PerformanceProbe.cs");
            return -1;
        }
    }

    private static bool IsBossLikeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return false;
        }

        return sceneName.StartsWith("GG_", StringComparison.Ordinal)
            || sceneName is "GG_Workshop" or "GG_Atrium" or "GG_Atrium_Roof";
    }

    private static string GetCurrentSceneName()
    {
        GameManager? gm = GameManager.instance;
        if (gm != null && !string.IsNullOrEmpty(gm.sceneName))
        {
            return gm.sceneName;
        }

        return USceneManager.GetActiveScene().name;
    }

    private static void OnUnityLog(string condition, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(condition))
        {
            return;
        }

        if (condition == "Couldn't find a Game Manager, make sure one exists in the scene.")
        {
            missingGameManagerLogCount++;
            if (missingGameManagerStackSamples < 3)
            {
                missingGameManagerStackSamples++;
                Log($"[PerfProbe] MissingGM sample#{missingGameManagerStackSamples} stack={FirstStackLine(stackTrace)}");
            }

            return;
        }

        if (condition == "Couldn't find a Hero, make sure one exists in the scene.")
        {
            missingHeroLogCount++;
            if (missingHeroStackSamples < 3)
            {
                missingHeroStackSamples++;
                Log($"[PerfProbe] MissingHero sample#{missingHeroStackSamples} stack={FirstStackLine(stackTrace)}");
            }
        }
    }

    private static string FirstStackLine(string stackTrace)
    {
        if (string.IsNullOrWhiteSpace(stackTrace))
        {
            return "<empty>";
        }

        string line = stackTrace
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "<empty>";

        return line.Trim();
    }
}
