namespace GodhomeQoL.Modules.QoL;

public sealed class UnlockRadiant : Module {
	public override bool DefaultEnabled => false;

	private protected override void Load() =>
		On.BossChallengeUI.Setup += HookSetup;

	private protected override void Unload() =>
		On.BossChallengeUI.Setup -= HookSetup;

	private static void HookSetup(
		On.BossChallengeUI.orig_Setup orig,
		BossChallengeUI self,
		BossStatue statue,
		string nameSheet,
		string nameKey,
		string descSheet,
		string descKey
	) {
		void invokeOrig() => orig(self, statue, nameSheet, nameKey, descSheet, descKey);

		if (statue.hasNoTiers) {
			invokeOrig();
			return;
		}

		BossStatue.Completion completion = statue.UsingDreamVersion ? statue.DreamStatueState : statue.StatueState;

		Unlock(invokeOrig, statue, ref completion);

		SetStatueCompletion(statue, completion);

		self.tier1Button.SetState(completion.completedTier1);
		self.tier2Button.SetState(completion.completedTier2);
		self.tier3Button.SetState(completion.completedTier3);
	}

	private static void Unlock(Action orig, BossStatue statue, ref BossStatue.Completion completion) {
		if (completion.completedTier2) {
			orig.Invoke();
			return;
		}

		completion.completedTier2 = true;
		completion.seenTier3Unlock = true;
		SetStatueCompletion(statue, completion);

		orig.Invoke();

		completion.completedTier2 = false;

		LogDebug($"Unlocked Radiant for {statue.name}");
	}

	private static void SetStatueCompletion(BossStatue statue, BossStatue.Completion completion) {
		if (statue.UsingDreamVersion) {
			statue.DreamStatueState = completion;
		} else {
			statue.StatueState = completion;
		}
	}
}
