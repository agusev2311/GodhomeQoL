using GodhomeQoL.Modules.Tools;
using UnityEngine.UI;
using QolCanvasUtil = GodhomeQoL.Modules.Tools.CanvasUtil;

namespace GodhomeQoL.Modules.QoL;

public sealed class InvincibleIndicator : Module
{
    public override bool DefaultEnabled => false;

    private const float InvincibleThresholdSeconds = 10f;
    private const string IndicatorText = "Invincible ON";
    private const int IndicatorSortOrder = 10060;
    private const int IndicatorFontSize = 34;

    private float invincibleTimer;
    private bool indicatorVisible;
    private InvincibleIndicatorDisplay? display;

    private protected override void Load()
    {
        display ??= new InvincibleIndicatorDisplay(IndicatorSortOrder);
        invincibleTimer = 0f;
        indicatorVisible = false;
        display.Hide();
        ModHooks.HeroUpdateHook += OnHeroUpdate;
    }

    private protected override void Unload()
    {
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        invincibleTimer = 0f;
        indicatorVisible = false;

        if (display != null)
        {
            display.Destroy();
            display = null;
        }
    }

    private void OnHeroUpdate()
    {
        if (!ShouldCheckHero())
        {
            ResetIndicator();
            return;
        }

        if (IsHeroInvincible())
        {
            invincibleTimer += Time.unscaledDeltaTime;
            if (!indicatorVisible && invincibleTimer >= InvincibleThresholdSeconds)
            {
                display?.Show(IndicatorText);
                indicatorVisible = true;
            }

            return;
        }

        ResetIndicator();
    }

    private void ResetIndicator()
    {
        invincibleTimer = 0f;
        if (indicatorVisible)
        {
            indicatorVisible = false;
            display?.Hide();
        }
    }

    private static bool ShouldCheckHero()
    {
        GameManager? manager = GameManager.instance;
        if (manager == null || manager.gameState != GameState.PLAYING)
        {
            return false;
        }

        return HeroController.instance != null;
    }

    private static bool IsHeroInvincible()
    {
        if (InvincibilityClaims.HasActiveClaims)
        {
            return true;
        }

        HeroController? hero = HeroController.instance;
        if (hero != null)
        {
            try
            {
                if (hero.cState.invulnerable)
                {
                    return true;
                }
            }
            catch (Exception swallowed)
            {
                LogSuppressed(swallowed, "InvincibleIndicator.cs");
            }
        }

        if (!IsHeroBoxActive())
        {
            return true;
        }

        PlayerData pd = PlayerData.instance;
        if (pd != null && pd.isInvincible)
        {
            return true;
        }

        return PlayerDataR.isInvincible;
    }

    private static bool IsHeroBoxActive()
    {
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            return true;
        }

        Transform heroBoxTransform = hero.transform.Find("HeroBox");
        if (heroBoxTransform == null)
        {
            return true;
        }

        return heroBoxTransform.gameObject.activeInHierarchy;
    }

    private sealed class InvincibleIndicatorDisplay
    {
        private readonly GameObject canvas;
        private readonly Text text;

        public InvincibleIndicatorDisplay(int sortOrder)
        {
            canvas = QolCanvasUtil.CreateCanvas(
                RenderMode.ScreenSpaceOverlay,
                new Vector2(1920f, 1080f),
                "InvincibleIndicatorCanvas",
                sortOrder
            );

            CanvasGroup group = canvas.GetComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;

            UObject.DontDestroyOnLoad(canvas);

            text = QolCanvasUtil.CreateTextPanel(
                canvas,
                string.Empty,
                IndicatorFontSize,
                TextAnchor.LowerLeft,
                new QolCanvasUtil.RectData(
                    new Vector2(400f, 60f),
                    new Vector2(260f, 914f),
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f),
                    new Vector2(0f, 0f)),
                QolCanvasUtil.GetFont("Perpetua")
            ).GetComponent<Text>();

            text.color = new Color(1f, 1f, 1f, 0.95f);
            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2f, -2f);
            outline.useGraphicAlpha = true;
            text.gameObject.SetActive(false);
        }

        public void Show(string message)
        {
            text.text = message;
            text.gameObject.SetActive(true);
            canvas.SetActive(true);
        }

        public void Hide()
        {
            text.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            UObject.Destroy(canvas);
        }
    }
}
