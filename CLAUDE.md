# CLAUDE.md

Hollow Knight mod (Modding API 1.5, .NET Framework 4.7.2, C# latest) for Godhome challenge runners. Fork of NightFuryoOo/GodhomeQoL. Install/build instructions for players: `INSTALL.md`.

## Building

The project references the game's DLLs (`$(HollowKnightRefs)` = a modded `hollow_knight_Data/Managed` with `Mods/Satchel`, `Mods/Osmi`, `Mods/Vasi`). They are not in the repo.

- CI: `.github/workflows/build.yml` (setup-hk downloads the refs; deps in `ModDependencies.txt`). It must run on `windows-latest`: setup-hk fetches the platform's `Assembly-CSharp`, and the Linux one lacks Windows-only types (GOG, XInput) the mod references.
- Locally without the game, reproduce CI: get `https://files.hk-modding.org/managed-linux.zip`, the Modding API **windows** zip (not linux, see above) from `hk-modding/modlinks` `ApiLinks.xml`, and Satchel/Osmi/Vasi from `ModLinks.xml` into one folder, then:
  `dotnet build GodhomeQoL.csproj -c Release -p:HollowKnightRefs=<dir>/ -p:ManagedModsDir=<tmp>/managed/ -p:ExportDir=<tmp>/export`
  Always override `ManagedModsDir`/`ExportDir`: the `CopyMod` target otherwise writes into the game folder and `C:\Users\User\Documents`.
- Expected warnings: unresolved `GalaxyCSharp`, `UnityEngine.ARModule` (unused).
- There are no automated tests. Behaviour can only be verified in game; say so when reporting.

## Layout

- `GodhomeQoL.cs`, `ModuleManager.cs`, `Modules/Module.cs` — entry point and module system.
- `Modules/<Feature>/` — one `Module` subclass per feature, often split into partial files (`*.Hooks.cs`, `*.Health.cs`, ...).
- `Modules/BossManipulate/` — per-boss helpers (HP, phase thresholds, summons) that patch boss FSMs.
- `Modules/QuickMenu/` — the F3 overlay UI (`QuickMenuController` partials); first-run defaults in `QuickMenuController.Defaults.cs` (`ApplyInitialDefaults`), panel "Reset defaults" buttons in `OverlayActions/`.
- `Settings/` — `GlobalSettings`/`LocalSettings` and the `[GlobalSetting]`/`[LocalSetting]` static-field serializer (`SettingBase`).
- `ToggleableBindings/` — embedded copy of the ToggleableBindings library. `Compatibility/*.cs` is excluded from compilation.
- `Resources/Lang/en.json` — UI strings (`"Modules/<Name>"`, `"Settings/<Name>"`).

## Module rules

- A module is loaded (`Load()`) only while `Enabled` (or `AlwaysEnabled`); `Unload()` must remove every hook it added.
- **Everything is opt-in.** New modules use `DefaultEnabled => false`, new settings default to off, and first-run/reset defaults must not enable anything.
- Do not hook the game while a feature is off. If a module must be `AlwaysEnabled` (has its own on/off setting in the UI), install hooks only while that setting is on — see the `SyncHooks()` pattern in `FreezeHitboxes`, `MaskDamage`, `NailDamageCheck`, `GearSwitcher.SyncRuntimeHooks()`.
- Non-hidden modules get an entry in `GlobalSettings.Modules`; per-save module state lives in `LocalSettings.PerSaveModules`.
- To force-reset existing users' settings, bump `CurrentDefaultsResetVersion` in `Settings/Settings.cs`.

## Conventions

- Match the surrounding file: some files use tabs, some spaces; some have a UTF-8 BOM and/or CRLF. Preserve both when editing.
- `GlobalUsings.cs` provides most namespaces (`Osmi`, `Satchel` helpers, `Module`, `Logger` statics such as `Log`, `LogDebug`, `LogSuppressed`).
- MonoMod hooks: `On.Type.Method += handler` / `IL.Type.Method += handler`; handlers are static.

## Known issues / background

- Vanilla bug: `SendRandomEventV3.loops` is never reset, so after ~100 rolls in one fight a boss always picks `events[0]` (e.g. Winged Nosk spamming the roof attack). Long fights (custom HP) make it visible; not caused by the mod.
- Boss helpers are not neutral when enabled with custom options off (e.g. Winged Nosk HP is floored to 1050 via `Math.Max(hp, Default...VanillaHp)` in ~15 helpers).
- Freeze Hitboxes: freeze-on-death mode (Any Hits off) does not trigger; see ReadMe "Known issues".
