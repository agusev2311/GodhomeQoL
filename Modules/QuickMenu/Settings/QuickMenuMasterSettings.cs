namespace GodhomeQoL.Settings;

public sealed class QuickMenuMasterSettings
{
    public bool BossChallengeEnabled { get; set; } = false;
    public bool BossChallengeHasSnapshot { get; set; }
    public bool BossChallengeSavedInfiniteChallenge { get; set; }
    public bool BossChallengeSavedRestartOnSuccess { get; set; }
    public bool BossChallengeSavedRestartAndMusic { get; set; }
    public bool BossChallengeSavedCarefreeMelody { get; set; }
    public int BossChallengeSavedCarefreeMelodyMode { get; set; } = -1;
    public bool BossChallengeSavedInfiniteGrimm { get; set; }
    public bool BossChallengeSavedInfiniteRadiance { get; set; }
    public bool BossChallengeSavedSegmentedP5 { get; set; }
    public bool BossChallengeSavedAddLifeblood { get; set; }
    public bool BossChallengeSavedAddSoul { get; set; }
    public bool BossChallengeSavedForceArriveAnimation { get; set; }

    public bool QolEnabled { get; set; } = false;
    public bool QolHasSnapshot { get; set; }
    public bool QolSavedFastDreamWarp { get; set; }
    public bool QolSavedShortDeath { get; set; }
    public bool QolSavedHallOfGods { get; set; }
    public bool QolSavedUnlockAllModes { get; set; }
    public bool QolSavedUnlockPantheons { get; set; }
    public bool QolSavedUnlockRadiance { get; set; }
    public bool QolSavedUnlockRadiant { get; set; }
    public bool QolSavedInvincibleIndicator { get; set; }
    public bool QolSavedScreenShake { get; set; }

    public bool MenuAnimEnabled { get; set; } = false;
    public bool MenuAnimHasSnapshot { get; set; }
    public bool MenuAnimSavedDoorDefaultBegin { get; set; }
    public bool MenuAnimSavedFasterLoads { get; set; }
    public bool MenuAnimSavedFastMenus { get; set; }
    public bool MenuAnimSavedFastText { get; set; }
    public bool MenuAnimSavedAutoSkip { get; set; }
    public bool MenuAnimSavedAllowSkipping { get; set; }
    public bool MenuAnimSavedSkipWithoutPrompt { get; set; }

    public bool BossAnimEnabled { get; set; } = false;
    public bool BossAnimHasSnapshot { get; set; }
    public bool BossAnimSavedHallOfGods { get; set; }
    public bool BossAnimSavedAbsoluteRadiance { get; set; }
    public bool BossAnimSavedPantheonVEnding { get; set; }
    public bool BossAnimSavedPureVesselRoar { get; set; }
    public bool BossAnimSavedGrimmNightmare { get; set; }
    public bool BossAnimSavedGreyPrinceZote { get; set; }
    public bool BossAnimSavedCollector { get; set; }
    public bool BossAnimSavedSoulMaster { get; set; }
    public bool BossAnimSavedCollectorRoar { get; set; }

    public bool RandomPantheonsEnabled { get; set; } = false;
    public bool RandomPantheonsHasSnapshot { get; set; }
    public bool RandomPantheonsSavedP1 { get; set; }
    public bool RandomPantheonsSavedP2 { get; set; }
    public bool RandomPantheonsSavedP3 { get; set; }
    public bool RandomPantheonsSavedP4 { get; set; }
    public bool RandomPantheonsSavedP5 { get; set; }

    public bool TrueBossRushEnabled { get; set; } = false;
    public bool TrueBossRushHasSnapshot { get; set; }
    public bool TrueBossRushSavedP1 { get; set; }
    public bool TrueBossRushSavedP2 { get; set; }
    public bool TrueBossRushSavedP3 { get; set; }
    public bool TrueBossRushSavedP4 { get; set; }
    public bool TrueBossRushSavedP5 { get; set; }

    public bool CheatsEnabled { get; set; } = false;
    public bool CheatsHasSnapshot { get; set; }
    public bool CheatsSavedInfiniteSoul { get; set; }
    public bool CheatsSavedInfiniteHp { get; set; }
    public bool CheatsSavedInvincibility { get; set; }
    public bool CheatsSavedNoclip { get; set; }

    public bool BossManipulateGlobalP5Enabled { get; set; }
    public List<string> BossManipulateGlobalP5TouchedModules { get; set; } = new();
    public List<string> BossManipulateGlobalP5EnabledModules { get; set; } = new();
}
