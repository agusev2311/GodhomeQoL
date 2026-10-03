namespace GodhomeQoL.Modules.QoL;

public sealed class DoorDefaultBegin : Module {
	public override bool DefaultEnabled => false;

	private protected override void Load() => On.BossDoorChallengeUI.ShowSequence += OnShowSequence;

	private protected override void Unload() => On.BossDoorChallengeUI.ShowSequence -= OnShowSequence;

	private static IEnumerator OnShowSequence(On.BossDoorChallengeUI.orig_ShowSequence orig, BossDoorChallengeUI self) {
		IEnumerator routine = orig(self);
		while (routine.MoveNext()) {
			yield return routine.Current;
		}

		yield return null;

		SelectBegin(self);
	}

	private static void SelectBegin(BossDoorChallengeUI self) {
		var beginButton = self.gameObject.Child("Panel", "BeginButton");
		EventSystem? eventSystem = EventSystem.current;
		InputHandler? inputHandler = InputHandler.Instance;
		if (eventSystem == null || inputHandler == null) {
			return;
		}

		if (beginButton != null) {
			eventSystem.SetSelectedGameObject(beginButton);
		}

		inputHandler.StartAcceptingInput();
		inputHandler.StartUIInput();
		TeleportManager.TryResetUiInput(beginButton);
		EnsureDoorUiInput();
	}

	internal static void TrySelectBeginAfterBindings(BossDoorChallengeUI? self) {
		if (self == null) {
			return;
		}

		if (!ModuleManager.TryGetLoadedModule<DoorDefaultBegin>(out _)) {
			return;
		}

		SelectBegin(self);
	}

	private static void EnsureDoorUiInput() {
		if (InputHandler.Instance == null || UIManager.instance == null || EventSystem.current == null) {
			return;
		}

		var inputModule = UIManager.instance.inputModule;
		var actions = InputHandler.Instance.inputActions;
		if (inputModule == null || actions == null) {
			return;
		}

		inputModule.SubmitAction = actions.menuSubmit;
		inputModule.CancelAction = actions.menuCancel;
		inputModule.MoveAction = actions.moveVector;
		inputModule.forceModuleActive = true;
		inputModule.ActivateModule();
	}
}
