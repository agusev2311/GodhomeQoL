namespace GodhomeQoL.Modules.BossChallenge;

// Hooks are installed only while at least one HalveDamage* module is enabled
internal static class HalveDamage {
	private static readonly List<Func<bool>> predicates = [];

	internal static void AddPredicate(Func<bool> predicate) {
		if (predicates.Count == 0) {
			ModHooks.TakeHealthHook += MakeDamageHalved;
			On.HeroController.StartRecoil += FixTakeHitEffect;
		}

		predicates.Add(predicate);
	}

	internal static void RemovePredicate(Func<bool> predicate) {
		if (!predicates.Remove(predicate) || predicates.Count > 0) {
			return;
		}

		ModHooks.TakeHealthHook -= MakeDamageHalved;
		On.HeroController.StartRecoil -= FixTakeHitEffect;
	}

	private static bool ShouldActivate() {
		foreach (Func<bool> predicate in predicates) {
			if (predicate.Invoke()) {
				return true;
			}
		}

		return false;
	}

	private static int MakeDamageHalved(int damage) =>
		ShouldActivate() ? (int) Math.Ceiling(damage / 2f) : damage;

	private static IEnumerator FixTakeHitEffect(On.HeroController.orig_StartRecoil orig, HeroController self, CollisionSide impactSide, bool spawnDamageEffect, int damageAmount) =>
		orig(self, impactSide, spawnDamageEffect, MakeDamageHalved(damageAmount));
}

public abstract class HalveDamageConditioned : Module {
	private Func<bool>? registeredPredicate;

	private protected sealed override void Load() {
		registeredPredicate ??= Predicate;
		HalveDamage.AddPredicate(registeredPredicate);
	}

	private protected sealed override void Unload() {
		if (registeredPredicate != null) {
			HalveDamage.RemovePredicate(registeredPredicate);
		}
	}

	private protected abstract bool Predicate();
}

public sealed class HalveDamageOtherPlace : HalveDamageConditioned {
	private protected override bool Predicate() => !BossSceneController.IsBossScene;
}

public sealed class HalveDamagePantheons : HalveDamageConditioned {
	private protected override bool Predicate() => BossSequenceController.IsInSequence;
}
