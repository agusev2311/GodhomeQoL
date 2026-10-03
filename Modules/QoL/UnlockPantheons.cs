namespace GodhomeQoL.Modules.QoL;

public sealed class UnlockPantheons : Module
{
    private static readonly (string goName, string prompt)[] atriumRoofObjects =
    {
        ("Breakable Wall_Silhouette", "Land of Storms shortcut"),
        ("GG Fall Platform", "Stepping stone platform"),
        ("gg_roof_lever", "Stepping stone platform lever"),
        ("Secret Mask", "Mask above spa")
    };

    public override bool DefaultEnabled => false;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ReloadSave;

    private protected override void Load() =>
        On.HeroController.Start += Unlock;

    private protected override void Unload() =>
        On.HeroController.Start -= Unlock;

    private static void Unlock(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);

        if (!PlayerDataR.bossRushMode)
        {
            return;
        }

        List<PersistentBoolData>? items = Ref.SD?.persistentBoolItems;

        if (!PlayerDataR.bossDoorCageUnlocked)
        {
            PlayerDataR.bossDoorCageUnlocked = true;
            LogDebug("P4 unlocked");
        }

        if (!PlayerDataR.finalBossDoorUnlocked)
        {
            PlayerDataR.finalBossDoorUnlocked = true;
            items?.Set("GG_Atrium", "gg_roof_lever", true);
            LogDebug("P5 unlocked");
        }

        if (items == null)
        {
            LogDebug("UnlockPantheons: persistentBoolItems not available yet, skipping roof object activation");
            return;
        }

        atriumRoofObjects.ForEach(tuple =>
        {
            if (!items.IsActivated("GG_Atrium_Roof", tuple.goName))
            {
                items.Set("GG_Atrium_Roof", tuple.goName, true);
                LogDebug($"{tuple.prompt} activated");
            }
        });
    }
}
