using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private void OnBossChallengeBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossChallengeVisible(false);
        }

        private void OnRandomPantheonsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetRandomPantheonsVisible(false);
        }

        private void OnRandomPantheonsResetDefaultsClicked()
        {
            randomPantheonsMasterEnabled = false;
            randomPantheonsMasterHasSnapshot = false;
            randomPantheonsSavedP1 = false;
            randomPantheonsSavedP2 = false;
            randomPantheonsSavedP3 = false;
            randomPantheonsSavedP4 = false;
            randomPantheonsSavedP5 = false;

            Modules.BossChallenge.RandomPantheons.Pantheon1Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon2Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon3Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon4Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon5Enabled = false;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(1);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(2);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(3);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(4);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(5);
            SetRandomPantheonsEnabled(false);
            RefreshRandomPantheonsUi();
            SaveMasterSettings();
        }

        private void OnTrueBossRushBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetTrueBossRushVisible(false);
        }

        private void OnTrueBossRushResetDefaultsClicked()
        {
            trueBossRushMasterEnabled = false;
            trueBossRushMasterHasSnapshot = false;
            trueBossRushSavedP1 = false;
            trueBossRushSavedP2 = false;
            trueBossRushSavedP3 = false;
            trueBossRushSavedP4 = false;
            trueBossRushSavedP5 = false;

            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled = false;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(1);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(2);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(3);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(4);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(5);
            SetTrueBossRushEnabled(false);
            RefreshTrueBossRushUi();
            SaveMasterSettings();
        }

        private void OnBossChallengeResetDefaultsClicked()
        {
            bossChallengeMasterEnabled = false;
            bossChallengeMasterHasSnapshot = false;
            SetInfiniteChallengeEnabled(false);
            SetCarefreeMelodyMode(Modules.QoL.CarefreeMelodyReset.ModeOff);
            SetForceArriveAnimationEnabled(false);
            SetInfiniteGrimmPufferfishEnabled(false);
            SetInfiniteRadianceClimbingEnabled(false);
            SetSegmentedP5Enabled(false);
            SetAddLifebloodEnabled(false);
            SetAddSoulEnabled(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;
            RefreshBossChallengeUi();
            SaveMasterSettings();
        }

        private void OnQolBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQolVisible(false);
        }

        private void OnQolResetDefaultsClicked()
        {
            qolMasterEnabled = true;
            qolMasterHasSnapshot = false;
            waitingForFastDreamWarpRebind = false;
            fastDreamWarpPrevKeyRaw = string.Empty;
            fastDreamWarpPrevButton = default;

            SetFastDreamWarpEnabled(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetShortDeathAnimationEnabled(false);
            SetUnlockAllModesEnabled(false);
            SetUnlockPantheonsEnabled(false);
            SetUnlockRadianceEnabled(false);
            SetUnlockRadiantEnabled(false);
            SetInvincibleIndicatorEnabled(false);
            SetScreenShakeEnabled(false);
            waitingForNailDamageCheckRebind = false;
            nailDamageCheckPrevKey = string.Empty;
            NailDamageCheck.SetKeybind(string.Empty);

            RefreshQolUi();
            SaveMasterSettings();
        }

        private void OnMenuAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetMenuAnimationVisible(false);
        }

        private void OnMenuAnimationResetDefaultsClicked()
        {
            menuAnimMasterEnabled = true;
            menuAnimMasterHasSnapshot = false;
            SetDoorDefaultBeginEnabled(false);
            SetFasterLoadsEnabled(false);
            SetFastMenusEnabled(false);
            SetFastTextEnabled(false);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = false;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = false;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = false;
            RefreshMenuAnimationUi();
            SaveMasterSettings();
        }

        private void OnBossAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetBossAnimationVisible(false);
        }

        private void OnBossAnimationResetDefaultsClicked()
        {
            bossAnimMasterEnabled = true;
            bossAnimMasterHasSnapshot = false;
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = false;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = false;
            Modules.QoL.SkipCutscenes.PantheonVEnding = false;
            Modules.QoL.SkipCutscenes.PureVesselRoar = false;
            Modules.QoL.SkipCutscenes.GrimmNightmare = false;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = false;
            Modules.QoL.SkipCutscenes.Collector = false;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = false;
            SetCollectorRoarEnabled(false);
            RefreshBossAnimationUi();
            SaveMasterSettings();
        }
    }
}
