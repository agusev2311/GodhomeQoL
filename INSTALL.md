# Installing GodhomeQoL

[Русская версия](INSTALL.ru.md)

GodhomeQoL is a [Hollow Knight Modding API](https://github.com/hk-modding/api) mod. It needs three library mods: **Satchel**, **Osmi** and **Vasi**.

> **Fork note.** Lumafly (the mod manager) installs the *official* GodhomeQoL from [NightFuryoOo/GodhomeQoL](https://github.com/NightFuryoOo/GodhomeQoL). Builds from this repository have to be installed by hand, as described below.

## 1. Back up your saves (recommended)

Saves and every mod's settings live in one folder:

```
%USERPROFILE%\AppData\LocalLow\Team Cherry\Hollow Knight
```

PowerShell one-liner that copies it to a dated folder on the Desktop:

```powershell
Copy-Item "$env:USERPROFILE\AppData\LocalLow\Team Cherry\Hollow Knight" "$([Environment]::GetFolderPath('Desktop'))\HK_backup_$(Get-Date -Format yyyy-MM-dd_HH-mm)" -Recurse
```

## 2. Install the Modding API and dependencies

Easiest: install [Lumafly](https://themulhima.github.io/Lumafly/), let it install the Modding API, then install **Satchel**, **Osmi** and **Vasi** from its mod list. Make sure all three are *enabled* — Lumafly moves disabled mods to `Mods\Disabled\`, and the game does not load those.

## 3. Get the mod DLL

Pick one:

- **Release:** download `GodhomeQoL.zip` from the repository's *Releases* page.
- **Latest build of any branch:** *Actions* tab → *Build* workflow → open a run → download the `GodhomeQoL` artifact (requires being logged in to GitHub).
- **Build it yourself:** see [Building from source](#building-from-source).

## 4. Copy it into the game

Put `GodhomeQoL.dll` here (default Steam path):

```
C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\GodhomeQoL\GodhomeQoL.dll
```

```powershell
$mods = "C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
New-Item -ItemType Directory -Path "$mods\GodhomeQoL" -Force | Out-Null
Copy-Item ".\GodhomeQoL.dll" "$mods\GodhomeQoL\" -Force
```

The game must be closed while replacing the file. If PowerShell reports *Access denied*, run it as administrator.

## 5. Check that it loaded

- The mod list in the top-right corner of the main menu shows `GodhomeQoL`.
- Press **F3** in game to open the quick menu.
- Errors go to `ModLog.txt` in the folder from step 1.

**Everything is off by default.** Nothing in the game changes until you enable a feature in the F3 menu. In the *Quality of Life*, *Boss Animation Skipping* and *Menu Animation Skipping* groups, first switch the group itself on (top row), then the individual options.

When updating from an older version, all GodhomeQoL settings are reset to "off" once (the log shows `Reset all modules to disabled (defaults migration)`).

## Uninstalling

Delete `Managed\Mods\GodhomeQoL\` and `GodhomeQoL.GlobalSettings.json` from the folder in step 1.
Note: *Unlock All Modes* writes the Steel Soul / Godseeker unlock into the game's own data; removing the mod does not undo it.

## Building from source

The project targets .NET Framework 4.7.2 and compiles against the game's DLLs, so it needs a modded `Managed` folder (Modding API + Satchel, Osmi, Vasi).

### With Docker (nothing installed on Windows except Docker Desktop)

Run from the repository folder (the one containing `GodhomeQoL.csproj`). The game folder is mounted read-only; output goes to `bin\Release\net472\GodhomeQoL.dll` and `out\export\GodhomeQoL\GodhomeQoL.zip`.

```powershell
docker run --rm `
  -v "${PWD}:/src" `
  -v "C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed:/hk:ro" `
  -w /src mcr.microsoft.com/dotnet/sdk:8.0 `
  dotnet build GodhomeQoL.csproj -c Release `
    -p:HollowKnightRefs=/hk/ `
    -p:ManagedModsDir=/src/out/managed/ `
    -p:ExportDir=/src/out/export
```

If you use Windows PowerShell 5.1 and pass a script to `bash -c`, avoid double quotes inside it — PowerShell strips them.

### With a local .NET SDK

```powershell
dotnet build GodhomeQoL.csproj -c Release -p:HollowKnightRefs="<path to hollow_knight_Data\Managed>\"
```

Without the extra properties the build also copies the DLL into `Managed\Mods\GodhomeQoL\` and writes an export to `C:\Users\User\Documents\HKModsExport` (see the `CopyMod` target in the `.csproj`).

### On GitHub Actions

`.github/workflows/build.yml` builds every push and pull request; pushing a tag like `v1.0.1.5` also publishes a GitHub Release. The game DLLs are downloaded by the [setup-hk](https://github.com/BadMagic100/setup-hk) action, and the mod dependencies come from `ModDependencies.txt`.
