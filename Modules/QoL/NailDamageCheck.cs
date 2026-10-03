namespace GodhomeQoL.Modules.Tools;

public sealed class NailDamageCheck : Module
{
    private const float DisplaySeconds = 3f;

    public override bool DefaultEnabled => true;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private static NailDamageCheckDisplay? display;
    private static float hideAtTime = -1f;

    private static bool moduleLoaded;
    private static bool hooksInstalled;

    private protected override void Load()
    {
        moduleLoaded = true;
        SyncHooks();
    }

    private protected override void Unload()
    {
        moduleLoaded = false;
        SyncHooks();
        DestroyDisplay();
    }

    // Only listen for input while a key is actually bound
    private static void SyncHooks()
    {
        bool shouldInstall = moduleLoaded && GetKey() != KeyCode.None;
        if (shouldInstall == hooksInstalled)
        {
            return;
        }

        hooksInstalled = shouldInstall;
        if (shouldInstall)
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }
        else
        {
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
        }
    }

    internal static string GetKeybind() => GodhomeQoL.GlobalSettings.NailDamageCheckKeybind ?? string.Empty;

    internal static void SetKeybind(string value)
    {
        GodhomeQoL.GlobalSettings.NailDamageCheckKeybind = value ?? string.Empty;
        GodhomeQoL.SaveGlobalSettingsSafe();
        SyncHooks();
    }

    internal static KeyCode GetKey()
    {
        string raw = GetKeybind();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return KeyCode.None;
        }

        return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
    }

    private static void OnHeroUpdate()
    {
        if (hideAtTime >= 0f && Time.unscaledTime >= hideAtTime)
        {
            HideDisplay();
        }

        if (QuickMenu.IsAnyUiVisible() || QuickMenu.IsHotkeyInputBlocked())
        {
            return;
        }

        KeyCode key = GetKey();
        if (key != KeyCode.None && Input.GetKeyDown(key))
        {
            ShowNailDamage();
        }
    }

    private static void ShowNailDamage()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        display ??= new NailDamageCheckDisplay();
        display.Display($"Nail Damage: {pd.nailDamage}");
        hideAtTime = Time.unscaledTime + DisplaySeconds;
    }

    private static void HideDisplay()
    {
        display?.Hide();
        hideAtTime = -1f;
    }

    private static void DestroyDisplay()
    {
        display?.Destroy();
        display = null;
        hideAtTime = -1f;
    }
}

internal sealed class NailDamageCheckDisplay
{
    private string displayText = "";
    private readonly Vector2 textSize = new(500, 100);
    private readonly Vector2 textPosition = new(0.985f, 0.965f);

    private GameObject? canvas;
    private UnityEngine.UI.Text? text;

    public NailDamageCheckDisplay() => Create();

    private void Create()
    {
        if (canvas != null)
        {
            return;
        }

        canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080), "NailDamageCheckCanvas");

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        UObject.DontDestroyOnLoad(canvas);

        text = CanvasUtil.CreateTextPanel(
            canvas,
            "",
            28,
            TextAnchor.UpperRight,
            new CanvasUtil.RectData(textSize, Vector2.zero, textPosition, textPosition, new Vector2(1f, 1f)),
            CanvasUtil.GetFont("Perpetua")
        ).GetComponent<UnityEngine.UI.Text>();
    }

    public void Destroy()
    {
        if (canvas != null)
        {
            UObject.Destroy(canvas);
        }

        canvas = null;
        text = null;
    }

    public void Hide()
    {
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (text == null || canvas == null)
        {
            return;
        }

        text.text = displayText;
        canvas.SetActive(true);
    }

    public void Display(string value)
    {
        displayText = value.Trim();
        Update();
    }
}
